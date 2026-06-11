using _4Paws.Common.Results;
using _4Paws.DTOs.Caregiver.Requests;
using _4Paws.Helper.CareGiver;
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
        private readonly ICurrentCareGiver _currentCareGiver;

        public CareGiverController(ICaregiverService caregiverService, ICurrentCareGiver currentCareGiver)
        {
            _caregiverService = caregiverService;
            _currentCareGiver = currentCareGiver;
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
            var careGiver = _currentCareGiver.GetCurrentCareGiver();
            if (careGiver == null) return Unauthorized("User profile not found.");

            var result = _caregiverService.GetCaregiverDashboard(careGiver.Id);
            return StatusCode(result.Status, result);
        }

        [HttpGet("MyListings")]
        public IActionResult GetMyListings()
        {
            var careGiver = _currentCareGiver.GetCurrentCareGiver();
            if (careGiver == null) return Unauthorized();

            var result = _caregiverService.GetCaregiverListings(careGiver.Id);
            return StatusCode(result.Status, result);
        }

        [HttpGet("MyAgreements")]
        public IActionResult GetMyAgreements()
        {
            var careGiver = _currentCareGiver.GetCurrentCareGiver();
            if (careGiver == null) return Unauthorized();

            var result = _caregiverService.GetCaregiverAgreements(careGiver.Id);
            return StatusCode(result.Status, result);
        }

        [AllowAnonymous]
        [HttpGet("Profile/{caregiverId}")]
        public IActionResult GetPublicProfile(int caregiverId)
        {
            var result = _caregiverService.GetCaregiverById(caregiverId);
            return StatusCode(result.Status, result);
        }


        [HttpPut("Bio")]
        public IActionResult UpdateBio([FromBody] string bio)
        {
            var careGiver = _currentCareGiver.GetCurrentCareGiver();
            if (careGiver == null) return Unauthorized();

            var result = _caregiverService.UpdateBio(careGiver.Id, bio);
            return StatusCode(result.Status, result);
        }
    }
}