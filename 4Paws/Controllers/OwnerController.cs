using _4Paws.Common.Results;
using _4Paws.DTOs.Caregiver.Requests;
using _4Paws.DTOs.Owner.Requests;
using _4Paws.DTOs.Owner.Responses;
using _4Paws.Helper.Owner;
using _4Paws.Services.CareGiver;
using _4Paws.Services.Owner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _4Paws.Controllers
{
    [Authorize]
    [Route("api/[controller]"), ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerService _ownerService;
        private readonly ICurrentOwner _currentOwner;

        public OwnerController(IOwnerService ownerService, ICurrentOwner currentOwner)
        {
            _ownerService = ownerService;
            _currentOwner = currentOwner;
        }

        [HttpPost]
        public IActionResult CreateOwnerProfile(CreateOwnerProfileRequest request)
        {
            var result = _ownerService.CreateOwnerProfile(request);
            return StatusCode(result.Status, result);
        }

        [HttpGet("Dashboard")]
        public IActionResult GetDashboard()
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null) return Unauthorized("User profile not found.");

            var result = _ownerService.GetOwnerDashboard(owner.Id);
            return StatusCode(result.Status, result);
        }

        [HttpGet("MyListings")]
        public IActionResult GetMyListings()
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null) return Unauthorized();

            var result = _ownerService.GetOwnerListings(owner.Id);
            return StatusCode(result.Status, result);
        }

        [HttpGet("MyAgreements")]
        public IActionResult GetMyAgreements()
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null) return Unauthorized();

            var result = _ownerService.GetOwnerAgreements(owner.Id);
            return StatusCode(result.Status, result);
        }

        [AllowAnonymous]
        [HttpGet("Profile/{ownerId}")]
        public IActionResult GetPublicProfile(int ownerId)
        {
            var result = _ownerService.GetOwnerById(ownerId);
            return StatusCode(result.Status, result);
        }
    }
}