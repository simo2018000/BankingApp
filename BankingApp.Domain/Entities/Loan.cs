using System;

namespace BankingApp.Domain
{
    public class Loan
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; } // Links loan to a specific account
        public decimal PrincipalAmount { get; set; }
        public double InterestRate { get; set; }
        public int TermInMonths { get; set; }
        public DateTime StartDate { get; set; }
        public string? Status { get; set; } = null; // e.g., "Pending", "Approved", "Paid"
    }
}