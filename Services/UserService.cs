    using BusinessObjects.Entities;
using Microsoft.AspNetCore.Identity;
using Repositories;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var users = _repository.GetAll(); // vẫn là IQueryable
            var result = await Task.Run(() => users.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                IsActive = u.IsActive,
                Role = u.Role,
                LastLoginAt = u.LastLoginAt
            }).ToList());

            return result;
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            return new UserResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                Role = user.Role,
                LastLoginAt = user.LastLoginAt,
            };
        }

        public async Task<UserResponseDto> AddUserAsync(UserRequestDto dto)
        {
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Role = dto.Role,
                IsActive = dto.IsActive,
            };

            user.Password = _passwordHasher.HashPassword(user, dto.Password);

            var created = await _repository.AddUserAsync(user);

            return new UserResponseDto
            {
                UserId = created.UserId,
                FullName = created.FullName,
                Email = created.Email,
                CreatedAt = created.CreatedAt,
                IsActive = created.IsActive,
                Role = created.Role,
                LastLoginAt = created.LastLoginAt
            };
        }

        public async Task UpdateUserAsync(UserRequestDto dto, int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Id is not found");

            if(dto.Password != null)
                existing.Password = _passwordHasher.HashPassword(existing, dto.Password);

            existing.FullName = dto.FullName;
            existing.Email = dto.Email;
            existing.Role = dto.Role;
            existing.IsActive = dto.IsActive;

            await _repository.UpdateUserAsync(existing);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _repository.DeleteUserAsync(id);
        }
    }
}
