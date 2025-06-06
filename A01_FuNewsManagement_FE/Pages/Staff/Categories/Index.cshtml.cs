using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.DTOs;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace A01_FuNewsManagement_FE.Pages.Staff.Categories
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public IndexModel(IHttpClientFactory httpClientFactory,
                         ILogger<IndexModel> logger,
                         IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _configuration = configuration;

            // Cấu hình base address và headers
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7281/";
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public List<CategoryResponseDto> Categories { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy danh sách Categories từ API");
                // Build the OData query based on filters
                var query = "api/Categories";
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(SearchString))
                {
                    queryParams.Add($"$filter=contains(CategoryName,'{SearchString}') or contains(Description,'{SearchString}')");
                }

                // Combine CategoryFilter and StatusFilter with 'and' if both are provided
                var filterConditions = new List<string>();
                           

                if (filterConditions.Any())
                {
                    queryParams.Add($"$filter={string.Join(" and ", filterConditions)}");
                }

                if (queryParams.Any())
                {
                    query += "?" + string.Join("&", queryParams);
                }

                var response = await _httpClient.GetFromJsonAsync<List<CategoryResponseDto>>(                   
                    query,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() } // Để handle AccountRole enum
                    }
                );

                if (response != null)
                {
                    Categories = response;
                    _logger.LogInformation("Lấy danh sách {Count} categories thành công", Categories.Count);
                }
                else
                {
                    _logger.LogWarning("API trả về response không hợp lệ: {Response}", response);
                    ErrorMessage = "Không thể tải danh sách người dùng. Vui lòng thử lại.";
                    Categories = new List<CategoryResponseDto>();
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Lỗi kết nối API khi lấy danh sách Categories");
                ErrorMessage = "Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.";
                Categories = new List<CategoryResponseDto>();
            }
            //catch (TaskCanceledException tcEx) when (tcEx.InnerException is TimeoutException)
            //{
            //    _logger.LogError(tcEx, "Timeout khi gọi API lấy danh sách Categories");
            //    ErrorMessage = "Yêu cầu quá thời gian. Vui lòng thử lại.";
            //    Categories = new List<UserResponseDto>();
            //}
            //catch (JsonException jsonEx)
            //{
            //    _logger.LogError(jsonEx, "Lỗi parse JSON khi lấy danh sách users");
            //    ErrorMessage = "Dữ liệu trả về không đúng định dạng. Vui lòng liên hệ admin.";
            //    Users = new List<UserResponseDto>();
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Lỗi không xác định khi lấy danh sách users");
            //    ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
            //    Users = new List<UserResponseDto>();
            //}

            return Page();
        }
    }
}
