using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> GetUserByIdAsync(int userId);
        Task<UserResponseDto> AddUserAsync(UserRequestDto dto);
        Task UpdateUserAsync(UserRequestDto dto, int id);
        Task DeleteUserAsync(int id);

    }
}
