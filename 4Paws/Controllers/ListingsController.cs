using _4Paws.Common.Results;
using _4Paws.DTOs.Listing.Requests;
using _4Paws.DTOs.Listing.Responses;
using _4Paws.Services.Listing;
using Microsoft.AspNetCore.Mvc;

namespace _4Paws.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class ListingsController : ControllerBase
    {
        private readonly IListingService _listingService;
        public ListingsController(IListingService listingService) => _listingService = listingService;

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _listingService.GetAllActiveListings();
            return StatusCode(result.Status, result);
        }

        [HttpGet("my-listings")]
        public IActionResult GetMyListings()
        {
            var result = _listingService.GetMyListings();
            return StatusCode(result.Status, result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _listingService.GetListingById(id);
            return StatusCode(result.Status, result);
        }

        [HttpPost]
        public IActionResult Create(CreateListingRequest req)
        {
            var result = _listingService.CreateListing(req);
            return StatusCode(result.Status, result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateListingRequest req)
        {
            var result = _listingService.UpdateListing(id, req);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _listingService.DeleteListing(id);
            return StatusCode(result.Status, result);
        }
    }
}
