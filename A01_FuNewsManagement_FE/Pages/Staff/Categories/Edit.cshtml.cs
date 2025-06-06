using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.DTOs;
using System.Text.Json;

namespace A01_FuNewsManagement_FE.Pages.Staff.Categories
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public CategoryRequestDto Category { get; set; }

        [BindProperty]
        public int CategoryId { get; set; }

        public List<SelectListItem> ParentCategories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // Lấy category đang được edit
            var response = await client.GetAsync($"api/Categories/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryResponseDto>>();
            if (apiResponse == null || apiResponse.result == null)
                return NotFound();

            var category = apiResponse.result;

            // Gán dữ liệu category vào form
            CategoryId = category.CategoryId;
            Category = new CategoryRequestDto
            {
                CategoryName = category.CategoryName,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId,
                IsActive = category.IsActive
            };

            // Lấy danh sách parent categories (trừ chính nó)
            var parentListResponse = await client.GetFromJsonAsync<List<CategoryResponseDto>>("api/Categories");
            if (parentListResponse != null)
            {
                ParentCategories = parentListResponse
                    .Where(c => c.CategoryId != id)
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
                return Page();

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PutAsJsonAsync($"api/Categories/{id}", Category);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            // Đọc lỗi từ response nếu có
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
                ModelState.AddModelError(string.Empty, rawContent);
            }

            // Load lại danh sách parent categories để không bị mất dropdown sau lỗi
            var parentListResponse = await client.GetFromJsonAsync<ApiResponse<List<CategoryResponseDto>>>("api/Categories");
            if (parentListResponse != null)
            {
                ParentCategories = parentListResponse.result
                    .Where(c => c.CategoryId != id)
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();
            }

            return Page();
        }
    }
}
