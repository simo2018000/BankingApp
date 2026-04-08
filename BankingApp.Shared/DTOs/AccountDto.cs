using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Remove or comment out the following line if 'BankingApp.Domain.Entities' does not exist or is not referenced
//using BankingApp.Domain.Entities;

namespace BankingApp.Shared.DTOs
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }      // <-- ADD THIS

        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}

