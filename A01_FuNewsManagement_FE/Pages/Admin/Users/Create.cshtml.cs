using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.DTOs;

namespace A01_FuNewsManagement_FE.Pages.Admin.Users
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public UserRequestDto User { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("api/Users", User);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            // Nếu lỗi thì đọc message trả về từ API (nếu có)
            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Lỗi tạo người dùng: {errorContent}");
            return Page();
        }
    }
}
