using _4Paws.Common.Results;
using _4Paws.Data;
using _4Paws.DTOs.Review.Requests;
using _4Paws.DTOs.Review.Responses;
using _4Paws.Enums;
using _4Paws.Helper.Services;
using _4Paws.Validators.Review;
using Microsoft.EntityFrameworkCore;

namespace _4Paws.Services.Review
{
    public class ReviewService : IReviewService
    {
        private readonly DataContext _db;
        private readonly ICurrentUserService _currentUser;

        public ReviewService(DataContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        // ── Create Review ─────────────────────────────────────────────────

        public Result<CreateReviewResponse> CreateReview(CreateReviewRequest req)
        {
            // 1. Validate request
            var validator = new CreateReviewValidator();
            var validation = validator.Validate(req);
            if (!validation.IsValid)
                return Result<CreateReviewResponse>.ValidationError(
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            // 2. Load agreement with all participants
            var agreement = _db.Agreements
                .Include(a => a.Owner).ThenInclude(o => o.User)
                .Include(a => a.CareGiver).ThenInclude(c => c.User)
                .FirstOrDefault(a => a.Id == req.AgreementId);

            if (agreement == null)
                return Result<CreateReviewResponse>.NotFound("Agreement not found.");

            // 3. Agreement must be Completed before reviewing
            if (agreement.Status != AgreementStatus.Completed)
                return Result<CreateReviewResponse>.BadRequest(
                    "You can only review a completed agreement.");

            // 4. Reviewer must be a participant of this agreement
            var userId = _currentUser.CurrentUserId();
            bool isOwner = agreement.Owner.UserId == userId;
            bool isCareGiver = agreement.CareGiver.UserId == userId;

            if (!isOwner && !isCareGiver)
                return Result<CreateReviewResponse>.Unauthorized();

            // 5. Cannot review yourself
            if (req.OwnerId.HasValue && agreement.Owner.UserId == userId)
                return Result<CreateReviewResponse>.BadRequest("You cannot review yourself.");

            if (req.CareGiverId.HasValue && agreement.CareGiver.UserId == userId)
                return Result<CreateReviewResponse>.BadRequest("You cannot review yourself.");

            // 6. Prevent duplicate review for same target in same agreement
            var alreadyReviewed = _db.Reviews.Any(r =>
                r.AgreementId == req.AgreementId &&
                r.ReviewerId == userId &&
                (r.OwnerId == req.OwnerId ||
                 r.CareGiverId == req.CareGiverId ||
                 r.PetId == req.PetId));

            if (alreadyReviewed)
                return Result<CreateReviewResponse>.BadRequest(
                    "You have already submitted a review for this target in this agreement.");

            // 7. Save the review
            var review = new Models.Review
            {
                AgreementId = req.AgreementId,
                ReviewerId = userId,
                Rating = req.Rating,
                Comment = req.Comment?.Trim(),
                OwnerId = req.OwnerId,
                CareGiverId = req.CareGiverId,
                PetId = req.PetId,
            };

            _db.Reviews.Add(review);
            _db.SaveChanges();

            // 8. Recalculate average rating on the target
            RecalculateRating(req);

            // 9. Load reviewer name for response
            var reviewer = _db.Users.Find(userId);

            return Result<CreateReviewResponse>.Ok(new CreateReviewResponse
            {
                Id = review.Id,
                AgreementId = review.AgreementId,
                ReviewerName = reviewer?.FullName ?? "Unknown",
                Rating = review.Rating,
                RatingLabel = GetRatingLabel(review.Rating),
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                OwnerId = review.OwnerId,
                CareGiverId = review.CareGiverId,
                PetId = review.PetId,
            });
        }

        // ── Get Reviews ───────────────────────────────────────────────────

        public Result<IEnumerable<CreateReviewResponse>> GetOwnerReviews(int ownerId)
        {
            var reviews = _db.Reviews
                .Include(r => r.Reviewer)
                .Where(r => r.OwnerId == ownerId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => MapToResponse(r))
                .ToList();

            return Result<IEnumerable<CreateReviewResponse>>.Ok(reviews);
        }

        public Result<IEnumerable<CreateReviewResponse>> GetCareGiverReviews(int careGiverId)
        {
            var reviews = _db.Reviews
                .Include(r => r.Reviewer)
                .Where(r => r.CareGiverId == careGiverId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => MapToResponse(r))
                .ToList();

            return Result<IEnumerable<CreateReviewResponse>>.Ok(reviews);
        }

        public Result<IEnumerable<CreateReviewResponse>> GetPetReviews(int petId)
        {
            var reviews = _db.Reviews
                .Include(r => r.Reviewer)
                .Where(r => r.PetId == petId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => MapToResponse(r))
                .ToList();

            return Result<IEnumerable<CreateReviewResponse>>.Ok(reviews);
        }

        // ── Private helpers ───────────────────────────────────────────────

        private void RecalculateRating(CreateReviewRequest req)
        {
            if (req.CareGiverId.HasValue)
            {
                var avg = _db.Reviews
                    .Where(r => r.CareGiverId == req.CareGiverId)
                    .Average(r => (double)r.Rating);

                var careGiver = _db.CareGivers.Find(req.CareGiverId);
                if (careGiver != null)
                {
                    careGiver.CareGiverRating = ConvertToRating(avg);
                    _db.SaveChanges();
                }
            }

            if (req.OwnerId.HasValue)
            {
                var avg = _db.Reviews
                    .Where(r => r.OwnerId == req.OwnerId)
                    .Average(r => (double)r.Rating);

                var owner = _db.Owners.Find(req.OwnerId);
                if (owner != null)
                {
                    owner.OwnerRating = ConvertToRating(avg);
                    _db.SaveChanges();
                }
            }

            if (req.PetId.HasValue)
            {
                var avg = _db.Reviews
                    .Where(r => r.PetId == req.PetId)
                    .Average(r => (double)r.Rating);

                var pet = _db.Pets.Find(req.PetId);
                if (pet != null)
                {
                    pet.PetRating = ConvertToRating(avg);
                    _db.SaveChanges();
                }
            }
        }

        private static Rating ConvertToRating(double avg) => avg switch
        {
            <= 1.5 => Rating.VeryBad,
            <= 2.5 => Rating.Bad,
            <= 3.5 => Rating.Average,
            <= 4.5 => Rating.Good,
            _ => Rating.Excellent,
        };

        private static string GetRatingLabel(Rating rating) => rating switch
        {
            Rating.VeryBad => "Very Bad",
            Rating.Bad => "Bad",
            Rating.Average => "Average",
            Rating.Good => "Good",
            Rating.Excellent => "Excellent",
            _ => "Unknown",
        };

        private static CreateReviewResponse MapToResponse(Models.Review r) => new()
        {
            Id = r.Id,
            AgreementId = r.AgreementId,
            ReviewerName = r.Reviewer?.FullName ?? "Unknown",
            Rating = r.Rating,
            RatingLabel = GetRatingLabel(r.Rating),
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            OwnerId = r.OwnerId,
            CareGiverId = r.CareGiverId,
            PetId = r.PetId,
        };
    }
}
