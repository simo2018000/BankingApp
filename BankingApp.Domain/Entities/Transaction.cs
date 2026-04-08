namespace BankingApp.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }   // FK to Account
        public required Account Account { get; set; }  // Navigation property

        public decimal Amount { get; set; }   // Positive or negative
        public string Type { get; set; } = string.Empty; // Deposit, Withdrawal, Transfer

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;
    }
}
