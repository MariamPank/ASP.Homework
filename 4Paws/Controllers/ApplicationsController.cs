using _4Paws.Common.Results;
using _4Paws.DTOs.Application.Requests;
using _4Paws.DTOs.Application.Responses;
using _4Paws.Enums;
using _4Paws.Services.Application;
using Microsoft.AspNetCore.Mvc;

namespace _4Paws.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _appService;
        public ApplicationsController(IApplicationService appService) => _appService = appService;

        [HttpPost("apply")]
        public IActionResult Apply(ApplyRequest req)
        {
            var result = _appService.ApplyToListing(req);
            return StatusCode(result.Status, result);
        }

        [HttpGet("listing/{listingId}")]
        public IActionResult GetByListing(int listingId)
        {
            var result = _appService.GetApplicationsForListing(listingId);
            return StatusCode(result.Status, result);
        }

        [HttpGet("my-applications")]
        public IActionResult GetMyApps()
        {
            var result = _appService.GetMyApplications();
            return StatusCode(result.Status, result);
        }

        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(int id, ApplicationStatus status)
        {
            var result = _appService.UpdateApplicationStatus(id, status);
            return StatusCode(result.Status, result);
        }
    }
}
