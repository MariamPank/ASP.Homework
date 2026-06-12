using _4Paws.Models;
using _4Paws.Enums;

namespace _4Paws.Tests.Utilities
{
    public static class TestDataBuilder
    {
        public static UserModel CreateTestUser(
            int id = 1,
            string username = "testuser",
            string email = "test@example.com",
            bool isVerified = true,
            bool isBanned = false)
        {
            return new UserModel
            {
                Id = id,
                FullName = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                IsVerified = isVerified,
                IsBanned = isBanned,
                Role = UserRole.User,
                VerificationCode = null
            };
        }

        public static Owner CreateTestOwner(int id = 1, int userId = 1)
        {
            return new Owner
            {
                Id = id,
                UserId = userId,
                OwnerRating = Rating.Average,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                User = CreateTestUser(userId)
            };
        }

        public static CareGiver CreateTestCareGiver(int id = 1, int userId = 1)
        {
            return new CareGiver
            {
                Id = id,
                UserId = userId,
                Bio = "Test caregiver",
                CareGiverRating = Rating.Average,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                User = CreateTestUser(userId)
            };
        }

        public static Agreement CreateTestAgreement(
            int id = 1,
            int ownerId = 1,
            int careGiverId = 2,
            int status = 2)
        {
            return new Agreement
            {
                Id = id,
                OwnerId = ownerId,
                CareGiverId = careGiverId,
                Status = (AgreementStatus)status,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Owner = CreateTestOwner(ownerId),
                CareGiver = CreateTestCareGiver(careGiverId, careGiverId)
            };
        }

        public static Review CreateTestReview(
            int id = 1,
            int reviewerId = 1,
            int agreementId = 1,
            Rating rating = Rating.Excellent,
            string comment = "Great service!")
        {
            return new Review
            {
                Id = id,
                ReviewerId = reviewerId,
                AgreementId = agreementId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now,
                Reviewer = new UserModel { Id = reviewerId, FullName = "Test Reviewer" }
            };
        }
    }
}