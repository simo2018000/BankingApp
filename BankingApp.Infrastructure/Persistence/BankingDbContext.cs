using BankingApp.Domain;
using BankingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Infrastructure.Persistence
{
    public class BankingDbContext : DbContext
    {
        public BankingDbContext(DbContextOptions<BankingDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Otp> Otps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---- User → Account relationship (no cascade delete)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Accounts)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- Account: unique account number
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.AccountNumber)
                .IsUnique();

            // ---- Money precision (important!)
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasColumnType("numeric(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("numeric(18,2)");

            modelBuilder.Entity<Loan>()
                .Property(l => l.PrincipalAmount)
                .HasColumnType("numeric(18,2)");

            // ---- OTP constraints
            modelBuilder.Entity<Otp>()
                .HasIndex(o => new { o.UserId, o.HashedCode }); // Prevents collisions

            modelBuilder.Entity<Otp>()
                .Property(o => o.ExpiresAt)
                .IsRequired();

            // Prevent cascade delete for OTP
            modelBuilder.Entity<Otp>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
