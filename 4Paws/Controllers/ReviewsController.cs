using _4Paws.DTOs.Review.Requests;
using _4Paws.Services.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _4Paws.Controllers
{
    [Authorize]
    [Route("api/[controller]"), ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // POST /api/Reviews
        // Leave a review — Agreement must be Completed, reviewer must be a participant
        [HttpPost]
        public IActionResult CreateReview(CreateReviewRequest req)
        {
            var result = _reviewService.CreateReview(req);
            return StatusCode(result.Status, result);
        }

        // GET /api/Reviews/owner/{ownerId}
        // Get all reviews for a specific Owner (public)
        [AllowAnonymous]
        [HttpGet("owner/{ownerId}")]
        public IActionResult GetOwnerReviews(int ownerId)
        {
            var result = _reviewService.GetOwnerReviews(ownerId);
            return StatusCode(result.Status, result);
        }

        // GET /api/Reviews/caregiver/{careGiverId}
        // Get all reviews for a specific CareGiver (public)
        [AllowAnonymous]
        [HttpGet("caregiver/{careGiverId}")]
        public IActionResult GetCareGiverReviews(int careGiverId)
        {
            var result = _reviewService.GetCareGiverReviews(careGiverId);
            return StatusCode(result.Status, result);
        }

        // GET /api/Reviews/pet/{petId}
        // Get all reviews for a specific Pet (public)
        [AllowAnonymous]
        [HttpGet("pet/{petId}")]
        public IActionResult GetPetReviews(int petId)
        {
            var result = _reviewService.GetPetReviews(petId);
            return StatusCode(result.Status, result);
        }
    }
}