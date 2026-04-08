namespace BankingApp.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }      // FK to User
        public  User? User { get; set; }        // Navigation property

        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0m;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // New: Transactions linked to this account
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
