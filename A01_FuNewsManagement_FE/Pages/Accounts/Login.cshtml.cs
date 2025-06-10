using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.DTOs;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace A01_FuNewsManagement_FE.Pages.Accounts
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var loginDto = new LoginRequestDto
            {
                Email = this.Email,
                Password = this.Password
            };

            var client = _httpClientFactory.CreateClient("ApiClient");

            try
            {
                var response = await client.PostAsJsonAsync("api/Auth/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();
                    var accessToken = apiResponse.result.AccessToken;
                    var refreshToken = apiResponse.result.RefreshToken;

                    // Giả sử bạn dùng Session để lưu access token
                    HttpContext.Session.SetString("AccessToken", accessToken);
                    HttpContext.Session.SetString("RefreshToken", refreshToken);

                    // Giải mã JWT để lấy email (nếu không muốn backend trả email riêng)
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadJwtToken(accessToken);

                    var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    // Lấy role
                    var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                    if (!string.IsNullOrEmpty(email))
                    {
                        HttpContext.Session.SetString("UserEmail", email);
                    }
                    if ( role == "Admin")
                    {
                        return RedirectToPage("/Admin/Users/Index");
                    }
                    else if (role == "Staff")
                    {
                        return RedirectToPage("/Articles/Index");
                    }
                    else
                    {
                        /*ErrorMessage = "Tài khoản không có quyền truy cập phù hợp.";
                        return Page();*/
                        return RedirectToPage("/Articles/IndexCus");
                    }
                }

                ErrorMessage = "Tài khoản hoặc mật khẩu không đúng.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi: {ex.Message}";
            }

            return Page();
        }
    }

}
