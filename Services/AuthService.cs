using BusinessObjects.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
            _jwtService = jwtService;

        }

        public async Task<User> FindOrCreateExternalUser(string email, string fullName)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null)
                return user;

            user = new User
            {
                Email = email,
                FullName = fullName,
                IsActive = true,
                Role = (int)AccountRole.Staff, 
                Password = "GoogleLogin" 
            };

            await _userRepository.AddUserAsync(user);
            return user;
        }


        public async Task<LoginResponseDto> Authenticate(LoginRequestDto request)
        {
            // Kiểm tra user có tồn tại không
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("User is not existed");

            // Kiểm tra user có active không
            if (!user.IsActive)
                throw new Exception("User is not active");
            // Verify password
            //var authenticated = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            //if (authenticated == PasswordVerificationResult.Failed)
            //    throw new Exception("Password is invalid");

            // Tạo claims cho JWT token
            // nơi lưu thông tin user muốn đem theo trong token

            if (user.Password != request.Password)
                throw new Exception("Password is invalid");

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

            // Generate Access Token
            var accessToken = _jwtService.GenerateAccessToken(claims); //Header + Payload (Claim) + Signature (Header + Payload + secretKey)

            // Generate Refresh Token
            var refreshToken = _jwtService.GenerateRefreshToken(user.UserId);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

        }

       
    }
}
