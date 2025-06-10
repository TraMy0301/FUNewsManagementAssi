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
        private readonly HttpClient _httpClient;
        private readonly ILogger<CreateModel> _logger;

        public EditModel(IHttpClientFactory httpClientFactory,HttpClient httpClient, ILogger<CreateModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClient;
            _logger = logger;
        }

        [BindProperty]
        public ArticleRequestDto Article { get; set; }

        [BindProperty]
        public string ArticleId { get; set; }

        public IEnumerable<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();
        public List<Category> Categories { get; set; }


        public async Task<IActionResult> OnGetAsync(string id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // Lấy dữ liệu bài báo
            var response = await client.GetAsync($"api/Articles/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ArticleResponseDto>>();
            if (apiResponse == null || apiResponse.result == null)
                return NotFound();

            var article = apiResponse.result;

            ArticleId = article.ArticleId;
            Article = new ArticleRequestDto
            {
                Title = article.Title,
                Headline = article.Headline,
                Content = article.Content,
                Source = article.Source,
                Status = article.Status,
                CategoryId = article.CategoryId ?? 0, 
                CreatedAt = article.CreatedAt,
                ModifiedAt = article.ModifiedAt
            };

            _logger.LogInformation("Bắt đầu lấy danh sách danh mục cho form tạo bài viết");
            var client1 = _httpClientFactory.CreateClient("ApiClient");

            var categoryResponse = await client.GetFromJsonAsync<List<Category>>(
                "api/Categories",
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (categoryResponse != null)
            {
                Categories = categoryResponse;

                CategoryOptions = categoryResponse.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),   
                    Text = c.CategoryName              
                }).ToList();
            }
            else
            {
                Categories = new List<Category>();
                CategoryOptions = new List<SelectListItem>();
            }


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string id = ArticleId;
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.PutAsJsonAsync($"api/Articles/{id}", Article);

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

            // Load lại category nếu có lỗi
            //var categoryResponse = await client.GetFromJsonAsync<ApiResponse<List<CategoryResponseDto>>>("api/Categories");
            //if (categoryResponse?.result != null)
            //{
            //    Categories = categoryResponse.result.Select(c => new SelectListItem
            //    {
            //        Value = c.CategoryId.ToString(),
            //        Text = c.CategoryName
            //    }).ToList();
            //}

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

    }
}
