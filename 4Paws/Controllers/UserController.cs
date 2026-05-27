using _4Paws.Common.Services;
using _4Paws.DTOs.User.Requests;
using _4Paws.Helper.Owner;
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
        private readonly ICurrentOwner _currentOwner;
        private readonly FileUploadService _fileUpload;

        public UsersController(
            IUserService userService,
            ICurrentOwner currentOwner,
            FileUploadService fileUpload)
        {
            _userService = userService;
            _currentOwner = currentOwner;
            _fileUpload = fileUpload;
        }

        [HttpGet("me")]
        public IActionResult GetMyProfile()
        {
            var user = _currentOwner.GetCurrentOwner();
            if (user == null) return Unauthorized();

            var result = _userService.GetById(user.Id);
            return StatusCode(result.Status, result);
        }

        [HttpPut("change-password")]
        public IActionResult ChangePassword(ChangePasswordRequest req)
        {
            var user = _currentOwner.GetCurrentOwner();
            if (user == null) return Unauthorized();

            var result = _userService.ChangePassword(user.Id, req);
            return StatusCode(result.Status, result);
        }

        [HttpPut("edit")]
        public IActionResult EditUser(EditUserRequest req)
        {
            var user = _currentOwner.GetCurrentOwner();
            if (user == null) return Unauthorized();

            var result = _userService.EditUser(user.Id, req);
            return StatusCode(result.Status, result);
        }

        [HttpDelete]
        public IActionResult DeleteAccount()
        {
            var user = _currentOwner.GetCurrentOwner();
            if (user == null) return Unauthorized();

            var result = _userService.DeleteUser(user.Id);
            return StatusCode(result.Status, result);
        }

        // PUT /api/Users/avatar
        // Upload or replace user avatar image
        [HttpPut("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            var user = _currentOwner.GetCurrentOwner();
            if (user == null) return Unauthorized();

            try
            {
                var url = await _fileUpload.SaveImageAsync(file, "avatars");
                var result = _userService.UpdateAvatar(user.UserId, url);
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
            var user = _currentOwner.GetCurrentOwner();
            if (user == null) return Unauthorized();

            var result = _userService.DeleteAvatar(user.UserId, _fileUpload);
            return StatusCode(result.Status, result);
        }
    }
}
