using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using Services;
using Services.DTOs;
using System.Security.Claims;

namespace A01_FuNewsManagament_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly IJwtService _jwtService;
        public AuthController(IAuthService service, IJwtService jwtService)
        {
            _service = service;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<ApiResponse<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            var result = await _service.Authenticate(request);
            return new ApiResponse<LoginResponseDto>(200, "Login successful", result);
        }
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleResponse), "Auth", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                return BadRequest(new ApiResponse(400, "External authentication failed"));

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                return BadRequest(new ApiResponse(400, "Email not found from external provider"));

            // Xử lý user: tạo nếu chưa tồn tại
            var user = await _service.FindOrCreateExternalUser(email, name);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

            var accessToken = _jwtService.GenerateAccessToken(claims);
            var refreshToken = _jwtService.GenerateRefreshToken(user.UserId);

            var redirectUrl = $"https://localhost:7054/Accounts/GoogleRedirect?access_token={accessToken}&refresh_token={refreshToken}";
            return Redirect(redirectUrl);
        }

    }

}
