using BankingApp.Application.Interfaces;
using BankingApp.Domain.Entities;
using BankingApp.Domain.Exceptions;
using BankingApp.Infrastructure.Persistence;
using BankingApp.Shared.DTOs;

public class AccountService : IAccountService
{
    private readonly BankingDbContext _context;
    private readonly IUserService _userService;

    public AccountService(BankingDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<AccountDto?> GetAccountByIdAsync(Guid id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null) return null;

        return new AccountDto
        {
            Id = account.Id,
            UserId = account.UserId,
            AccountNumber = account.AccountNumber,
            Balance = account.Balance,
            IsActive = account.IsActive,
            CreatedAt = account.CreatedAt
        };
    }

    public async Task<AccountDto> CreateAccountAsync(AccountDto accountDto)
    {
        // Load user from DB (required)
        var user = await _context.Users.FindAsync(accountDto.UserId);
        if (user == null)
            throw new NotFoundException("User", accountDto.UserId);

        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = accountDto.UserId,
            User = user, // <-- REQUIRED

            AccountNumber = accountDto.AccountNumber,
            Balance = accountDto.Balance,
            IsActive = accountDto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return new AccountDto
        {
            Id = account.Id,
            UserId = account.UserId,
            AccountNumber = account.AccountNumber,
            Balance = account.Balance,
            IsActive = account.IsActive,
            CreatedAt = account.CreatedAt
        };
    }



    public async Task<bool> UpdateAccountAsync(Guid id, AccountDto accountDto)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null) return false;

        account.Balance = accountDto.Balance;
        account.IsActive = accountDto.IsActive;
        account.AccountNumber = accountDto.AccountNumber;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAccountAsync(Guid id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null) return false;

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
        return true;
    }
}
