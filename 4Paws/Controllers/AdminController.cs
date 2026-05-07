using _4Paws.Helper.Adm;
using _4Paws.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _4Paws.Controllers
{
    [Authorize]
    [Route("api/[controller]"), ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAdministrator _administrator;

        public AdminController(IAdminService adminService, IAdministrator administrator)
        {
            _adminService = adminService;
            _administrator = administrator;
        }

        // ── Stats ─────────────────────────────────────────────────────────

        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            if (!_administrator.IsAdmin()) return Forbid();
            var result = _adminService.GetStats();
            return StatusCode(result.Status, result);
        }

        // ── Users ─────────────────────────────────────────────────────────

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            if (!_administrator.IsAdmin()) return Forbid();
            var result = _adminService.GetAllUsers();
            return StatusCode(result.Status, result);
        }

        [HttpGet("users/{userId}")]
        public IActionResult GetUserById(int userId)
        {
            if (!_administrator.IsAdmin()) return Forbid();
            var result = _adminService.GetUserById(userId);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("users/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            if (!_administrator.IsAdmin()) return Forbid();
            var result = _adminService.DeleteUser(userId);
            return StatusCode(result.Status, result);
        }

        [HttpPut("users/{userId}/ban")]
        public IActionResult BanUser(int userId)
        {
            if (!_administrator.IsAdmin()) return Forbid();
            var result = _adminService.BanUser(userId);
            return StatusCode(result.Status, result);
        }

        [HttpPut("users/{userId}/unban")]
        public IActionResult UnbanUser(int userId)
        {
            if (!_administrator.IsAdmin()) return Forbid();
            var result = _adminService.UnbanUser(userId);
            return StatusCode(result.Status, result);
        }

        // ── Listings ──────────────────────────────────────────────────────

        [HttpGet("listings")]
        public IActionResult GetAllListings()
        {
            if (!_administrator.IsAdmin()) return Forbid();
            var result = _adminService.GetAllListings();
            return StatusCode(result.Status, result);
        }

        [HttpDelete("listings/{listingId}")]
        public IActionResult DeleteListing(int listingId)
        {
            if (!_administrator.IsAdmin()) return Forbid();
            var result = _adminService.DeleteListing(listingId);
            return StatusCode(result.Status, result);
        }

        // ── Applications ──────────────────────────────────────────────────

        [HttpGet("applications")]
        public IActionResult GetAllApplications()
        {
            if (!_administrator.IsAdmin()) return Forbid();
            var result = _adminService.GetAllApplications();
            return StatusCode(result.Status, result);
        }

        // ── Agreements ────────────────────────────────────────────────────

        [HttpGet("agreements")]
        public IActionResult GetAllAgreements()
        {
            if (!_administrator.IsAdmin()) return Forbid();
            var result = _adminService.GetAllAgreements();
            return StatusCode(result.Status, result);
        }
    }
}
