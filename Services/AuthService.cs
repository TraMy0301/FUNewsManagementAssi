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
            var authenticated = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (authenticated == PasswordVerificationResult.Failed)
                throw new Exception("Password is invalid");

            // Tạo claims cho JWT token
            // nơi lưu thông tin user muốn đem theo trong token
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

        //public async Task Logout(LogoutRequestDto request)
        //{
        //    // Validate refresh token format
        //    if (string.IsNullOrWhiteSpace(request.RefreshToken))
        //    {
        //        throw new AppException(ErrorCode.TOKEN_REQUIRED);
        //    }

        //    // Kiểm tra token đã bị blacklist chưa trước khi validate
        //    var isAlreadyBlacklisted = await _tokenBlacklistService.IsTokenBlacklistedAsync(request.RefreshToken);
        //    if (isAlreadyBlacklisted)
        //    {
        //        throw new AppException(ErrorCode.TOKEN_ALREADY_BLACKLISTED);
        //    }

        //    ClaimsPrincipal principal;
        //    try
        //    {
        //        // Validate refresh token sử dụng method có sẵn
        //        principal = _jwtService.ValidateRefreshToken(request.RefreshToken);
        //    }
        //    catch (SecurityTokenExpiredException)
        //    {
        //        throw new AppException(ErrorCode.TOKEN_EXPIRED);
        //    }
        //    catch (SecurityTokenException ex)
        //    {
        //        if (ex.Message.Contains("Invalid token type"))
        //        {
        //            throw new AppException(ErrorCode.INVALID_TOKEN_TYPE);
        //        }
        //        throw new AppException(ErrorCode.INVALID_REFRESH_TOKEN);
        //    }
        //    catch (Exception)
        //    {
        //        throw new AppException(ErrorCode.INVALID_REFRESH_TOKEN);
        //    }

        //    // Lấy thông tin từ token
        //    var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        //    {
        //        throw new AppException(ErrorCode.INVALID_REFRESH_TOKEN);
        //    }

        //    // Lấy JTI (JWT ID) từ token để blacklist
        //    var jtiClaim = principal.FindFirst(JwtRegisteredClaimNames.Jti);
        //    if (jtiClaim == null)
        //    {
        //        throw new AppException(ErrorCode.INVALID_REFRESH_TOKEN);
        //    }

        //    // Lấy expiry date từ token
        //    var expClaim = principal.FindFirst("exp");
        //    DateTime expiryDate = DateTime.UtcNow.AddDays(30); // Default fallback

        //    if (expClaim != null && long.TryParse(expClaim.Value, out long exp))
        //    {
        //        expiryDate = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;
        //    }

        //    // Thêm refresh token vào blacklist
        //    await _tokenBlacklistService.BlacklistTokenByJwtIdAsync(jtiClaim.Value, expiryDate);

        //}
    }
}
