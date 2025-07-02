using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.DTOs;
using System.Text.Json;
using BusinessObjects.Entities;
using Microsoft.AspNetCore.SignalR;
using Services;

namespace A01_FuNewsManagement_FE.Pages.Articles
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CreateModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public CreateModel(IHttpClientFactory httpClientFactory,
                          ILogger<CreateModel> logger,
                          IConfiguration configuration,
                          IWebHostEnvironment env)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _configuration = configuration;

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7281/";
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _env = env;
        }

        [BindProperty]
        public ArticleRequestDto Article { get; set; } = new()
        {
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            Status = "Draft"
        };

        public List<Category> Categories { get; set; } = new();

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }
        public List<Tag> Tags { get; set; } = new();

        [BindProperty]
        public IFormFile? UploadImage { get; set; }




        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy danh sách danh mục cho form tạo bài viết");

                var categoryResponse = await _httpClient.GetFromJsonAsync<List<Category>>(
                    "api/Categories",
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (categoryResponse != null)
                {
                    Categories = categoryResponse;
                    _logger.LogInformation("Danh sách danh mục: {Categories}", JsonSerializer.Serialize(Categories));
                }
                else
                {
                    _logger.LogWarning("API trả về danh sách danh mục không hợp lệ");
                    Categories = new List<Category>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách danh mục");
                ErrorMessage = "Không thể tải danh mục. Vui lòng thử lại.";
            }
            await LoadTagsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState không hợp lệ: {Errors}",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                await LoadCategoriesAsync();
                await LoadTagsAsync();
                return Page();
            }

            try
            {
                _logger.LogInformation("Bắt đầu tạo bài viết mới");

                // 1. Upload ảnh nếu có
                if (UploadImage != null && UploadImage.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(UploadImage.FileName)}";
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                    // Tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await UploadImage.CopyToAsync(stream);
                    }

                    // Gán đường dẫn tương đối để dùng trong trình duyệt
                    Article.ImageURL = $"/uploads/{fileName}";

                    _logger.LogInformation("Ảnh đã được lưu tại: {Path}", filePath);
                }
                else
                {
                    _logger.LogInformation("Không có ảnh nào được tải lên.");
                }

                // 2. Gửi dữ liệu bài viết (gồm ImageUrl) đến API
                var jsonContent = JsonSerializer.Serialize(Article, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                _logger.LogInformation("Dữ liệu gửi đi: {Json}", jsonContent);

                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/Articles/add", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Tạo bài viết thành công");
                    SuccessMessage = "Bài viết đã được tạo thành công.";
                    return RedirectToPage("./Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("API trả về lỗi khi tạo bài viết: {Error}", errorContent);
                    ErrorMessage = $"Không thể tạo bài viết: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo bài viết");
                ErrorMessage = "Có lỗi xảy ra khi tạo bài viết. Vui lòng thử lại.";
            }

            await LoadCategoriesAsync();
            await LoadTagsAsync();

            return Page();
        }



        private async Task LoadCategoriesAsync()
        {
            var categoryResponse = await _httpClient.GetFromJsonAsync<List<Category>>(
                "odata/Categories",
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            if (categoryResponse != null)
            {
                Categories = categoryResponse;
            }
        }

        private async Task LoadTagsAsync()
        {
            try
            {
                var tagResponse = await _httpClient.GetFromJsonAsync<List<Tag>>(
                    "odata/Tags",
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                Console.WriteLine("danh sách tag:"+tagResponse);

                Tags = tagResponse ?? new List<Tag>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể tải danh sách Tags");
                Tags = new List<Tag>();
            }
        }
    }
}