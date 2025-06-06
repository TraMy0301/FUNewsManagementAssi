using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Services;
using Services.DTOs;

namespace A01_FuNewsManagament_API.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        // GET: api/Users
        [EnableQuery]
        [HttpGet]
        public async Task<ActionResult<List<UserResponseDto>>> GetAll()
        {
            var users = await _service.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetUserById(int id)
        {
            try
            {
                var user = await _service.GetUserByIdAsync(id);
                return Ok(new ApiResponse<UserResponseDto>(200, "Lấy thông tin người dùng thành công", user));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<UserResponseDto>(404, ex.Message));
            }
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> AddUser([FromBody] UserRequestDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Dữ liệu không hợp lệ"));

            var response = await _service.AddUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = response.UserId },
                new ApiResponse<UserResponseDto>(201, "Tạo người dùng thành công", response));
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateUser(int id, [FromBody] UserRequestDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Dữ liệu không hợp lệ"));

            try
            {
                await _service.UpdateUserAsync(user, id);
                return Ok(new ApiResponse(200, "Cập nhật người dùng thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteUser(int id)
        {
            try
            {
                var user = await _service.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new ApiResponse(404, "Người dùng không tồn tại"));

                await _service.DeleteUserAsync(id);
                return Ok(new ApiResponse(200, "Xóa người dùng thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
        }
    }
}
