using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace A01_FuNewsManagement_FE.Pages.Articles
{
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public ArticleResponseDto Article { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)

        {

            // Giả sử bạn dùng Session để lưu access token
            var accessToken = HttpContext.Session.GetString("AccessToken");
            Console.WriteLine(accessToken);

            // Giải mã JWT để lấy email (nếu không muốn backend trả email riêng)
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(accessToken);

            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            ViewData["Role"] = role;
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7281/api/Articles/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<ApiResponse<ArticleResponseDto>>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Article = json?.Result;

            return Page();
        }

        public class ApiResponse<T>
        {
            public T Result { get; set; }
            public int Code { get; set; }
            public string Message { get; set; }
        }
    }

}
