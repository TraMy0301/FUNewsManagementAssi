using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using Services;
using Services.DTOs;

namespace A01_FuNewsManagament_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public async Task<ApiResponse<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            var result = await _service.Authenticate(request);
            return new ApiResponse<LoginResponseDto>(200, "Login successful", result);
        }
    }
}
