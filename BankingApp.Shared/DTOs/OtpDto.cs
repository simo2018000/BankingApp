using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Shared.DTOs
{
    public class OtpDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }   // expiry time
        public bool IsUsed { get; set; } = false;

        // Navigation property
        public required UserDto User { get; set; }
    }
}

