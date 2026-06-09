using _4Paws.Common.Services;
using _4Paws.DTOs.Pet.Requests;
using _4Paws.Helper.Owner;
using _4Paws.Services.Pet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _4Paws.Controllers
{
    [Authorize]
    [Route("api/[controller]"), ApiController]
    public class PetController : ControllerBase
    {
        private readonly IPetService _petService;
        private readonly ICurrentOwner _currentOwner;
        private readonly FileUploadService _fileUpload;

        public PetController(
            IPetService petService,
            ICurrentOwner currentOwner,
            FileUploadService fileUpload)
        {
            _petService = petService;
            _currentOwner = currentOwner;
            _fileUpload = fileUpload;
        }

        [HttpPost]
        public IActionResult Create(CreatePetRequest req)
        {
            var result = _petService.CreatePet(req);
            return StatusCode(result.Status, result);
        }

        [HttpGet("myPets")]
        public IActionResult GetMyPets()
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null) return Unauthorized();

            var result = _petService.GetMyPets();
            return StatusCode(result.Status, result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _petService.GetById(id);
            return StatusCode(result.Status, result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdatePetRequest req)
        {
            var result = _petService.UpdatePet(id, req);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _petService.DeletePet(id);
            return StatusCode(result.Status, result);
        }

        // PUT /api/Pets/{id}/image
        // Upload or replace pet image
        [HttpPut("{id}/image")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            try
            {
                var url = await _fileUpload.SaveImageAsync(file, "pets");
                var result = _petService.UpdatePetImage(id, url, _fileUpload);
                return StatusCode(result.Status, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/Pets/{id}/image
        // Remove pet image
        [HttpDelete("{id}/image")]
        public IActionResult DeleteImage(int id)
        {
            var result = _petService.DeletePetImage(id, _fileUpload);
            return StatusCode(result.Status, result);
        }
    }
}
