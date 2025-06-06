using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.DTOs;

namespace A01_FuNewsManagement_FE.Pages.Staff.Categories
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public CategoryRequestDto Category { get; set; } = new();

        public List<SelectListItem> ParentCategories { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetFromJsonAsync<List<CategoryResponseDto>>("api/Categories");

            if (response != null)
            {
                ParentCategories = response
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    })
                    .ToList();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("api/Categories", Category);

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

