using BankingApp.Application.Interfaces;
using BankingApp.Domain.Entities;
using BankingApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private readonly BankingDbContext _context;

        public OtpService(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateOtpAsync(Guid userId)
        {
            string otp = new Random().Next(100000, 999999).ToString();

            var hashedOtp = HashOtp(otp);

            var entity = new Otp
            {
                UserId = userId,
                HashedCode = hashedOtp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            _context.Otps.Add(entity);
            await _context.SaveChangesAsync();

            Log.Information("SECURITY_EVENT: OTP Generated | UserId={UserId}", userId);

            return otp;
        }

        public async Task<bool> ValidateOtpAsync(Guid userId, string otp)
        {
            var entity = await _context.Otps
                .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsUsed);

            if (entity == null) return false;
            if (entity.ExpiresAt < DateTime.UtcNow) return false;
            if (entity.HashedCode != HashOtp(otp)) return false;

            entity.IsUsed = true;
            await _context.SaveChangesAsync();

            Log.Information("SECURITY_EVENT: OTP Validated | UserId={UserId}", userId);
            return true;
        }

        public async Task InvalidateOtpAsync(Guid userId)
        {
            var entity = await _context.Otps
                .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsUsed);

            if (entity != null)
            {
                entity.IsUsed = true;
                await _context.SaveChangesAsync();

                Log.Warning("SECURITY_EVENT: OTP Invalidated | UserId={UserId}", userId);
            }
        }

        private string HashOtp(string otp)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(otp));
            return Convert.ToBase64String(bytes);
        }
    }
}
