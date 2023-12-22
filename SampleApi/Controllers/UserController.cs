using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace SampleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public User GetUser(int userId)
        {
            return _userService.GetUserById(userId);
        }

        [HttpPost("addUser")]
        public IActionResult AddUser([FromBody] CreateUserDto createUserDto)
        {
            // Thực hiện xác thực, kiểm tra dữ liệu đầu vào và xử lý lỗi nếu cần
            // ...

            // Gọi phương thức trong IUserService để thêm mới người dùng
            _userService.AddUser(createUserDto.Name);

            return Ok("User added successfully.");
        }

        [HttpPut("updateUser/{userId}")]
        public IActionResult UpdateUser(int userId, [FromBody] UpdateUserDto updateUserDto)
        {
            // Thực hiện xác thực, kiểm tra dữ liệu đầu vào và xử lý lỗi nếu cần
            // ...

            // Gọi phương thức trong IUserService để cập nhật người dùng
            _userService.UpdateUser(userId, updateUserDto.Name);

            return Ok($"User with ID {userId} updated successfully.");
        }

        [HttpDelete("deleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            // Gọi phương thức trong IUserService để xóa người dùng
            _userService.DeleteUser(userId);

            return Ok($"User with ID {userId} deleted successfully.");
        }
    }
}
