using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.DTOs;
using System.Text.Json;

namespace A01_FuNewsManagement_FE.Pages.Admin.Users
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public UserResponseDto User { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Users/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var content = await response.Content.ReadFromJsonAsync<ApiResponse<UserResponseDto>>();
            User = content?.result;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"api/Users/{User.UserId}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            // Nếu lỗi, đọc lỗi và hiển thị
            string raw = await response.Content.ReadAsStringAsync();
            try
            {
                var error = JsonSerializer.Deserialize<ApiResponse>(raw, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                ModelState.AddModelError(string.Empty, error?.message ?? "Có lỗi xảy ra.");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, raw);
            }

            return Page();
        }
    }
}
