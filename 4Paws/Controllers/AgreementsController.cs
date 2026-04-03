using _4Paws.Common.Results;
using _4Paws.DTOs.Agreement.Responses;
using _4Paws.Services.Agreement;
using Microsoft.AspNetCore.Mvc;

namespace _4Paws.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class AgreementsController : ControllerBase
    {
        private readonly IAgreementService _agreementService;
        public AgreementsController(IAgreementService agreementService) => _agreementService = agreementService;

        [HttpPost("create/{applicationId}")]
        public IActionResult Create(int applicationId)
        {
            var result = _agreementService.CreateAgreement(applicationId);
            return StatusCode(result.Status, result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _agreementService.GetAgreementById(id);
            return StatusCode(result.Status, result);
        }

        [HttpGet("my-agreements")]
        public IActionResult GetMyAgreements()
        {
            var result = _agreementService.GetMyAgreements();
            return StatusCode(result.Status, result);
        }

        [HttpPut("{id}/complete")]
        public IActionResult Complete(int id)
        {
            var result = _agreementService.CompleteAgreement(id);
            return StatusCode(result.Status, result);
        }

    }
}
