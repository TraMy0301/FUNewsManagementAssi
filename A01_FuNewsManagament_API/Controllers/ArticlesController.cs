using BusinessObjects.Entities;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.SignalR;
using Services;
using Services.DTOs;
using System.Threading.Tasks;

namespace A01_FuNewsManagament_API.Controllers
{
    //[Authorize(Roles = "Admin,Staff")]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _service;
        private readonly IHubContext<NotificationHub> _context;

        public ArticlesController(IArticleService service, IHubContext<NotificationHub> context)
        {
            _service = service;
            _context = context;
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<Article> Get()
        {
            return _service.GetArticles();
        }

       
        [EnableQuery]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ArticleResponseDto>>> GetArticleById(string id)
        {
            try
            {
                var article = await _service.GetArticleById(id);
                return Ok(new ApiResponse<ArticleResponseDto>(200, "Lấy thông tin người dùng thành công", article));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<ArticleResponseDto>(404, ex.Message));
            }
        }

        [EnableQuery]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ArticleResponseDto>>> AddArticle([FromBody] ArticleRequestDto article)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Dữ liệu không hợp lệ"));

            var response = await _service.AddArticle(article);
            await _context.Clients.All.SendAsync("ReceiveUpdate", article);
            return StatusCode(201, new ApiResponse<ArticleResponseDto>(201, "Tạo bài báo thành công", response));

        }

        [EnableQuery]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateArticle(string id, [FromBody] ArticleRequestDto article)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Dữ liệu không hợp lệ"));

            try
            {
                await _service.UpdateArticle(id, article);
                return Ok(new ApiResponse(200, "Cập nhật bài báo thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
        }

        [EnableQuery]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteArticle(string id)
        {
            try
            {
                var article = await _service.GetArticleById(id);
                if (article == null)
                    return NotFound(new ApiResponse(404, "Bài báo không tồn tại"));

                await _service.DeleteArticle(id);
                return Ok(new ApiResponse(200, "Xóa bài báo thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Lỗi: {ex.Message}"));
            }
        }
    }
}
