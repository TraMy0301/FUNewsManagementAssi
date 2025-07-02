using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.DTOs;
using System.Net.Http;
using System.Text.Json;

namespace A01_FuNewsManagement_FE.Pages.Articles
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CreateModel> _logger;

        public EditModel(IHttpClientFactory httpClientFactory, ILogger<CreateModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty]
        public ArticleRequestDto Article { get; set; } = new();

        [BindProperty]
        public string ArticleId { get; set; } = string.Empty;

        public List<Category> Categories { get; set; } = new();
        public List<Tag> Tags { get; set; } = new(); // 👉 Danh sách Tag để hiển thị checkbox
        [BindProperty]
        public IFormFile? UploadImage { get; set; }


        public IEnumerable<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();

        // 🟢 GET: Hiển thị form với dữ liệu bài viết cần sửa
        public async Task<IActionResult> OnGetAsync(string id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // 🧠 Lấy thông tin bài viết từ API
            var response = await client.GetAsync($"api/Articles/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ArticleResponseDto>>();
            if (apiResponse?.result == null)
                return NotFound();

            var article = apiResponse.result;

            ArticleId = article.ArticleId;

            // 🔁 Gán thông tin bài viết vào ArticleRequestDto để bind lên form
            Article = new ArticleRequestDto
            {
                Title = article.Title,
                Headline = article.Headline,
                Content = article.Content,
                Source = article.Source,
                Status = article.Status,
                CategoryId = article.CategoryId ?? 0,
                CreatedAt = article.CreatedAt,
                ModifiedAt = article.ModifiedAt,
                ImageURL = article.ImageURL,
                TagIds = article.Tags?.Select(t => t.TagId).ToList() ?? new List<int>() // ✅ Gán tag đã chọn
            };


            // 🔁 Load Category & Tag để hiển thị lên UI
            await LoadCategoriesAsync();
            await LoadTagsAsync();

            return Page();
        }

        // 🔁 POST: Gửi dữ liệu đã chỉnh sửa để cập nhật bài viết
        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                await LoadTagsAsync();
                return Page();
            }
            if (UploadImage != null && UploadImage.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(UploadImage.FileName)}";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadImage.CopyToAsync(stream);
                }

                // ✅ Gán đường dẫn ảnh vào DTO gửi API
                Article.ImageURL = $"/uploads/{fileName}";
            }



            var response = await client.PutAsJsonAsync($"api/Articles/{ArticleId}", Article);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("./Index");

            string rawContent = await response.Content.ReadAsStringAsync();
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse>(rawContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                ModelState.AddModelError(string.Empty, errorResponse?.message ?? "Có lỗi xảy ra.");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, rawContent);
            }

            await LoadCategoriesAsync();
            await LoadTagsAsync();

            return Page();
        }

        private async Task LoadCategoriesAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var categoryResponse = await client.GetFromJsonAsync<List<Category>>("api/Categories");

            Categories = categoryResponse ?? new List<Category>();
            CategoryOptions = Categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();
        }

        private async Task LoadTagsAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var tagResponse = await client.GetFromJsonAsync<List<Tag>>("odata/Tags");

            Tags = tagResponse ?? new List<Tag>();
        }
    }
}
