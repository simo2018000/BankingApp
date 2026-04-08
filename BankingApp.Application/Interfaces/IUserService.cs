using BankingApp.Shared.DTOs;

namespace BankingApp.Application.Interfaces
{
    public interface IUserService
    {
        
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<UserDto> CreateUserAsync(UserDto user);
        Task<bool> UpdateUserAsync(Guid id, UserDto user);
        Task<bool> DeleteUserAsync(Guid id);

    }
}
