using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.DTOs;
using System.Text.Json;
using System.Text;
using BusinessObjects.Entities;
using System.Net.Http;

namespace A01_FuNewsManagement_FE.Pages.Admin.Users
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public UserRequestDto User { get; set; }

        [BindProperty]
        public int UserId { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Users/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserResponseDto>>();
            if (apiResponse == null || apiResponse.result == null)
                return NotFound();

            var user = apiResponse.result;
            UserId = user.UserId;
            User = new UserRequestDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {

            if (!ModelState.IsValid)
                return Page();

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PutAsJsonAsync($"api/Users/{id}", User);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            // Đọc content trả về (nếu có) và cố gắng parse JSON an toàn
            string rawContent = await response.Content.ReadAsStringAsync();

            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse>(rawContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                ModelState.AddModelError(string.Empty, errorResponse?.message ?? "Có lỗi xảy ra.");
            }
            catch (JsonException)
            {
                // Nếu không phải JSON hợp lệ, gán thẳng nội dung trả về vào lỗi
                ModelState.AddModelError(string.Empty, rawContent);
            }

            return Page();
        }



    }
}
