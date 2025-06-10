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

        public CreateModel(IHttpClientFactory httpClientFactory,
                          ILogger<CreateModel> logger,
                          IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _configuration = configuration;

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7281/";
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
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

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                await LoadCategoriesAsync();
                return Page();
            }

            try
            {
                _logger.LogInformation("Bắt đầu tạo bài viết mới");

                var jsonContent = JsonSerializer.Serialize(Article, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                _logger.LogInformation("Dữ liệu gửi đi: {JsonContent}", jsonContent);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/Articles", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Tạo bài viết thành công");
                    SuccessMessage = "Bài viết đã được tạo thành công.";
                    return RedirectToPage("./Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("API trả về lỗi khi tạo bài viết: StatusCode={StatusCode}, Error={Error}", response.StatusCode, errorContent);
                    ErrorMessage = $"Không thể tạo bài viết: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo bài viết");
                ErrorMessage = "Có lỗi xảy ra khi tạo bài viết. Vui lòng thử lại.";
            }

            await LoadCategoriesAsync();

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
    }
}