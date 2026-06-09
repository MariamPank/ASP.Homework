using _4Paws.Common.Services;
using _4Paws.DTOs.User.Requests;
using _4Paws.Helper.Services;
using _4Paws.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _4Paws.Controllers
{
    [Authorize]
    [Route("api/[controller]"), ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUser;
        private readonly FileUploadService _fileUpload;

        public UsersController(
            IUserService userService,
            ICurrentUserService currentUser,
            FileUploadService fileUpload)
        {
            _userService = userService;
            _currentUser = currentUser;
            _fileUpload = fileUpload;
        }

        [HttpGet("me")]
        public IActionResult GetMyProfile()
        {
            var userId = _currentUser.CurrentUserId();
            if (userId == 0) return Unauthorized();
            var result = _userService.GetById(userId);
            return StatusCode(result.Status, result);
        }

        [HttpPut("change-password")]
        public IActionResult ChangePassword(ChangePasswordRequest req)
        {
            var userId = _currentUser.CurrentUserId();
            if (userId == 0) return Unauthorized();
            var result = _userService.ChangePassword(userId, req);
            return StatusCode(result.Status, result);
        }

        [HttpPut("edit")]
        public IActionResult EditUser(EditUserRequest req)
        {
            var userId = _currentUser.CurrentUserId();
            if (userId == 0) return Unauthorized();
            var result = _userService.EditUser(userId, req);
            return StatusCode(result.Status, result);
        }

        [HttpDelete]
        public IActionResult DeleteAccount()
        {
            var userId = _currentUser.CurrentUserId();
            if (userId == 0) return Unauthorized();
            var result = _userService.DeleteUser(userId);
            return StatusCode(result.Status, result);
        }

        // PUT /api/Users/avatar
        // Upload or replace user avatar image
        [HttpPut("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            var userId = _currentUser.CurrentUserId();
            if (userId == 0) return Unauthorized();

            try
            {
                var url = await _fileUpload.SaveImageAsync(file, "avatars");
                var result = _userService.UpdateAvatar(userId, url);
                return StatusCode(result.Status, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/Users/avatar
        // Remove user avatar
        [HttpDelete("avatar")]
        public IActionResult DeleteAvatar()
        {
            var userId = _currentUser.CurrentUserId();
            if (userId == 0) return Unauthorized();
            var result = _userService.DeleteAvatar(userId, _fileUpload);
            return StatusCode(result.Status, result);
        }
    }
}
