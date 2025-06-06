using BusinessObjects.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Services;
using Services.DTOs;

namespace A01_FuNewsManagament_API.Controllers
{
    //[Authorize(Roles = "Staff")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        //[EnableQuery]
        //[HttpGet]
        //public async Task<ActionResult<List<CategoryResponseDto>>> GetAll()
        //{
        //    var categories = await _service.GetAllCategoriesAsync();
        //    return Ok(categories);
        //}


        [EnableQuery]
        public IQueryable<Category> Get()
        {
            return _service.GetAllEntities();    
        }

        [HttpGet("({key})")]
        [EnableQuery]
        public SingleResult<Category> Get([FromODataUri] int key)
        {

            var result = _service.GetAllEntities()
                                        .Where(a => a.CategoryId == key);

            return SingleResult.Create(result);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> GetById(int id)
        {
            try
            {
                var category = await _service.GetCategoryByIdAsync(id);
                return Ok(new ApiResponse<CategoryResponseDto>(200, "Lấy danh mục thành công", category));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<CategoryResponseDto>(404, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> AddCategory([FromBody] CategoryRequestDto category)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Dữ liệu không hợp lệ"));

            var response = await _service.AddCategoryAsync(category);
            return CreatedAtAction(nameof(GetById), new { id = response.CategoryId },
                new ApiResponse<CategoryResponseDto>(201, "Tạo danh mục thành công", response));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateCategory(int id, [FromBody] CategoryRequestDto category)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Dữ liệu không hợp lệ"));

            try
            {
                await _service.UpdateCategoryAsync(id, category);
                return Ok(new ApiResponse(200, "Cập nhật danh mục thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteCategory(int id)
        {
            try
            {
                var category = await _service.GetCategoryByIdAsync(id);
                if (category == null)
                    return NotFound(new ApiResponse(404, "Danh mục không tồn tại"));

                await _service.DeleteCategoryAsync(id);
                return Ok(new ApiResponse(200, "Xóa danh mục thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
        }
    }
}
