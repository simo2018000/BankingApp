using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankingApp.Application.Interfaces;
using BankingApp.Domain.Entities;
using BankingApp.Infrastructure.Persistence;
using BankingApp.Shared.DTOs;

namespace BankingApp.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly BankingDbContext _context;

        public UserService(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Username = userDto.Username,
                PhoneNumber = userDto.PhoneNumber,
                Email = userDto.Email,
                Role = userDto.Role,
                IsActive = userDto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> UpdateUserAsync(Guid id, UserDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Username = userDto.Username;
            user.PhoneNumber = userDto.PhoneNumber;
            user.Email = userDto.Email;
            user.IsActive = userDto.IsActive;
            // Role is intentionally excluded — use a dedicated admin endpoint to change roles

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
