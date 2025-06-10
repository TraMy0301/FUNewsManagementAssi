using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace A01_FuNewsManagement_FE.Pages.Articles
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public CategoryResponseDto Category { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7281/api/Categories/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var parsed = JsonConvert.DeserializeObject<ApiResponse<CategoryResponseDto>>(apiResponse);
                    Category = parsed.Data;
                    return Page();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ErrorMessage = "Không tìm thấy danh mục.";
                    return Page();
                }

                ErrorMessage = "Lỗi khi lấy thông tin danh mục.";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi kết nối: " + ex.Message;
                return Page();
            }
        }

        public class ApiResponse<T>
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }

        public class CategoryResponseDto
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}