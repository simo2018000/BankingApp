using BankingApp.Application.Interfaces;
using BankingApp.Domain.Exceptions;
using BankingApp.Infrastructure.Persistence;
using BankingApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        new CompactJsonFormatter(),
        "logs/bankapi.json",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankingApp API", Version = "v1" });
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

    builder.Services.AddDbContext<BankingDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IAccountService, AccountService>();
    builder.Services.AddScoped<IOtpService, OtpService>();
    builder.Services.AddScoped<IUserService, UserService>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = "https://localhost:5001";
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false
            };
        });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

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

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "L'application a crashed au démarrage");
}
finally
{
    Log.CloseAndFlush();
}