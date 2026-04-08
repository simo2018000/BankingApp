using BankingApp.Shared.DTOs;

public interface IAccountService
{
    Task<AccountDto?> GetAccountByIdAsync(Guid id);
    Task<AccountDto> CreateAccountAsync(AccountDto accountDto);
    Task<bool> UpdateAccountAsync(Guid id, AccountDto accountDto);
    Task<bool> DeleteAccountAsync(Guid id);
}
