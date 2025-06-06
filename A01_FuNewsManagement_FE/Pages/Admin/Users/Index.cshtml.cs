using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace A01_FuNewsManagement_FE.Pages.Admin.Users
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

        public List<UserResponseDto> Users { get; set; } = new();

        [TempData]
        public string? ErrorMessage { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy danh sách users từ API");

                var response = await _httpClient.GetFromJsonAsync<List<UserResponseDto>>(
                    "api/Users",
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() } // Để handle AccountRole enum
                    }
                );

                if (response != null)
                {
                    Users = response;
                    _logger.LogInformation("Lấy danh sách {Count} users thành công", Users.Count);
                }
                else
                {
                    _logger.LogWarning("API trả về response không hợp lệ: {Response}", response);
                    ErrorMessage = "Không thể tải danh sách người dùng. Vui lòng thử lại.";
                    Users = new List<UserResponseDto>();
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Lỗi kết nối API khi lấy danh sách users");
                ErrorMessage = "Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.";
                Users = new List<UserResponseDto>();
            }
            //catch (TaskCanceledException tcEx) when (tcEx.InnerException is TimeoutException)
            //{
            //    _logger.LogError(tcEx, "Timeout khi gọi API lấy danh sách users");
            //    ErrorMessage = "Yêu cầu quá thời gian. Vui lòng thử lại.";
            //    Users = new List<UserResponseDto>();
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