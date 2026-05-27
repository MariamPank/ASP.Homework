using _4Paws.Common.Results;
using _4Paws.DTOs.Review.Requests;
using _4Paws.DTOs.Review.Responses;

namespace _4Paws.Services.Review
{
    public interface IReviewService
    {
        Result<CreateReviewResponse> CreateReview(CreateReviewRequest req);
        Result<IEnumerable<CreateReviewResponse>> GetOwnerReviews(int ownerId);
        Result<IEnumerable<CreateReviewResponse>> GetCareGiverReviews(int careGiverId);
        Result<IEnumerable<CreateReviewResponse>> GetPetReviews(int petId);
    }
}
