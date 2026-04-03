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

        public UsersController(IUserService userService, ICurrentOwner currentOwner)
        {
            _userService = userService;
            _currentOwner = currentOwner;
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
    }
}
