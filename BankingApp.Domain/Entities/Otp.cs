using System;

namespace BankingApp.Domain.Entities
{
    public class Otp
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        // Hashed OTP stored in DB
        public string HashedCode { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}
