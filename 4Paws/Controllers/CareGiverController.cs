using _4Paws.Common.Results;
using _4Paws.DTOs.Caregiver.Requests;
using _4Paws.Helper.Owner;
using _4Paws.Services.CareGiver;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _4Paws.Controllers
{
    [Authorize]
    [Route("api/[controller]"), ApiController]
    public class CareGiverController : ControllerBase
    {
        private readonly ICaregiverService _caregiverService;
        private readonly ICurrentOwner _currentOwner;

        public CareGiverController(ICaregiverService caregiverService, ICurrentOwner currentOwner)
        {
            _caregiverService = caregiverService;
            _currentOwner = currentOwner;
        }

        [HttpPost]
        public IActionResult CreateCaregiverProfile(CreateCaregiverProfileRequest request)
        {
            var result = _caregiverService.CreateCaregiverProfile(request);
            return StatusCode(result.Status, result);
        }

        [HttpGet("Dashboard")]
        public IActionResult GetDashboard()
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null) return Unauthorized("User profile not found.");

            var result = _caregiverService.GetCaregiverDashboard(owner.Id);
            return StatusCode(result.Status, result);
        }

        [HttpGet("MyListings")]
        public IActionResult GetMyListings()
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null) return Unauthorized();

            var result = _caregiverService.GetCaregiverListings(owner.Id);
            return StatusCode(result.Status, result);
        }

        [HttpGet("MyAgreements")]
        public IActionResult GetMyAgreements()
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null) return Unauthorized();

            var result = _caregiverService.GetCaregiverAgreements(owner.Id);
            return StatusCode(result.Status, result);
        }

        [AllowAnonymous]
        [HttpGet("Profile/{caregiverId}")]
        public IActionResult GetPublicProfile(int caregiverId)
        {
            var result = _caregiverService.GetCaregiverById(caregiverId);
            return StatusCode(result.Status, result);
        }
    }
}