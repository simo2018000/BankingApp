using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Shared.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }   // FK to Account
        public required AccountDto Account { get; set; }  // Navigation property

        public decimal Amount { get; set; }   // Positive or negative
        public string Type { get; set; } = string.Empty; // Deposit, Withdrawal, Transfer

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;
    

    }
}
