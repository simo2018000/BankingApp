using System;
using System.Threading.Tasks;

namespace BankingApp.Application.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(Guid userId);
        Task<bool> ValidateOtpAsync(Guid userId, string otp);
        Task InvalidateOtpAsync(Guid userId);
    }
}
