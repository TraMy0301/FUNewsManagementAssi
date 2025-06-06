using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs;
using Services;
using BusinessObjects.Entities;
using Azure;
using Microsoft.AspNetCore.OData.Query;

namespace A01_FuNewsManagament_API.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _service;

        public TagsController(ITagService service)
        {
            _service = service;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<ActionResult<List<TagResponseDto>>> GetAll()
        {
            var tags = await _service.GetAllTags();
            return Ok(tags);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TagResponseDto>>> GetTagById(int id)
        {
            try
            {
                var tag = await _service.GetTagById(id);
                return Ok(new ApiResponse<TagResponseDto>(200, "Lấy thông tin tag thành công", tag));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<TagResponseDto>(404, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TagResponseDto>>> AddTag([FromBody] TagRequestDto tag)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Dữ liệu không hợp lệ"));

            var response = await _service.AddTag(tag);
            return CreatedAtAction(nameof(GetTagById), new { id = response.TagId },
                new ApiResponse<TagResponseDto>(201, "Tạo tag thành công", response));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateTag(int id, [FromBody] TagRequestDto tag)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Dữ liệu không hợp lệ"));

            try
            {
                await _service.UpdateTag(id, tag);
                return Ok(new ApiResponse(200, "Cập nhật tag bài báo thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteTag(int id)
        {
            try
            {
                var tag = await _service.GetTagById(id);
                if (tag == null)
                    return NotFound(new ApiResponse(404, "Tag không tồn tại"));

                await _service.DeleteTag(id);
                return Ok(new ApiResponse(200, "Xóa tag bài báo thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
        }
    }
}

