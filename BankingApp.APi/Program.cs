using BankingApp.Domain.Exceptions;

using Bank  ingApp.Application.Interfaces;
using BankingApp.Infrastructure.Persistence;
using BankingApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;

// --- 1. CONFIGURATION SERILOG (SIEM) ---
// On configure le logger pour écrire en JSON dans le dossier logs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        new CompactJsonFormatter(), // Format JSON critique pour QRadar
        "logs/bankapi.json",        // Chemin du fichier
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // --- 2. ACTIVER SERILOG ---
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankingApp API", Version = "v1" });

        // Configuration Swagger pour JWT (Optionnel mais pratique)
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new string[] { }
            }
        });
    });

    // Database Context
    builder.Services.AddDbContext<BankingDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Services Métiers
    builder.Services.AddScoped<IAccountService, AccountService>();
    builder.Services.AddScoped<IOtpService, OtpService>();
    builder.Services.AddScoped<IUserService, UserService>();

    // Authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            // Configuration JWT basique pour le dev
            options.Authority = "https://localhost:5001"; // A adapter selon votre config
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false // A sécuriser en prod !
            };
        });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            context.Response.ContentType = "application/json";

            if (error?.Error is NotFoundException)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(new { error = error.Error.Message });
            }
            else
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
            }
        });
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "L'application a crashé au démarrage");
}
finally
{
    Log.CloseAndFlush();
}