using BusinessObjects.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
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

        public ArticlesController(IArticleService service)
        {
            _service = service;
        }

        [EnableQuery]
        //[HttpGet]
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
            //return CreatedAtAction(nameof(GetArticleById), new { id = response.ArticleId },
            //    new ApiResponse<ArticleResponseDto>(201, "Tạo người dùng thành công", response));
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
        // DELETE: api/Users/5
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
        }
    }
}
