using Azure.Core;
using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.DTOs;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace A01_FuNewsManagement_FE.Pages.Articles
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient; // gọi API
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public IndexModel(IHttpClientFactory httpClientFactory,
                         ILogger<IndexModel> logger,
                         IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _configuration = configuration;

            // Configure base address and headers
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7281/";
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public List<ArticleResponseDto> Articles { get; set; } = new();
        public List<Category> Categories { get; set; } = new(); // For category dropdown

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string CategoryFilter { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; } = string.Empty;

        [TempData]
        public string? ErrorMessage { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy danh sách bài viết từ API");
                


                // Fetch categories for the dropdown
                var categoryResponse = await _httpClient.GetFromJsonAsync<List<Category>>(
                    "api/Categories",
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (categoryResponse != null)
                {
                    Categories = categoryResponse;
                }
                else
                {
                    _logger.LogWarning("API trả về danh sách danh mục không hợp lệ");
                    Categories = new List<Category>();
                }

                // Build the OData query based on filters
                var query = "api/Articles";
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(SearchString))
                {
                    queryParams.Add($"$filter=contains(Title,'{SearchString}') or contains(Content,'{SearchString}')");
                }

                // Combine CategoryFilter and StatusFilter with 'and' if both are provided
                var filterConditions = new List<string>();
                if (!string.IsNullOrEmpty(CategoryFilter))
                {
                    filterConditions.Add($"CategoryId eq {CategoryFilter}");
                }
                if (!string.IsNullOrEmpty(StatusFilter))
                {
                    filterConditions.Add($"Status eq '{StatusFilter}'");
                }

                if (filterConditions.Any())
                {
                    queryParams.Add($"$filter={string.Join(" and ", filterConditions)}");
                }

                if (queryParams.Any())
                {
                    query += "?" + string.Join("&", queryParams);
                }

                // Fetch articles with OData query
                var response = await _httpClient.GetFromJsonAsync<List<ArticleResponseDto>>(
                    query,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    }
                );

                if (response != null)
                {
                    Articles = response;
                    _logger.LogInformation("Lấy danh sách {Count} bài viết thành công", Articles.Count);
                }
                else
                {
                    _logger.LogWarning("API trả về danh sách bài viết không hợp lệ");
                    ErrorMessage = "Không thể tải danh sách bài viết. Vui lòng thử lại.";
                    Articles = new List<ArticleResponseDto>();
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Lỗi kết nối API khi lấy danh sách bài viết");
                ErrorMessage = "Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.";
                Articles = new List<ArticleResponseDto>();
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Lỗi parse JSON khi lấy danh sách bài viết");
                ErrorMessage = "Dữ liệu trả về không đúng định dạng. Vui lòng liên hệ admin.";
                Articles = new List<ArticleResponseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi lấy danh sách bài viết");
                ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                Articles = new List<ArticleResponseDto>();
            }

            return Page();
        }
    }
}