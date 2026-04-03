using _4Paws.DTOs.Pet.Requests;
using _4Paws.Helper.Owner;
using _4Paws.Services.Pet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _4Paws.Controllers
{
    [Authorize]
    [Route("api/[controller]"), ApiController]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;
        private readonly ICurrentOwner _currentOwner;

        public PetsController(IPetService petService, ICurrentOwner currentOwner)
        {
            _petService = petService;
            _currentOwner = currentOwner;
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
    }
}