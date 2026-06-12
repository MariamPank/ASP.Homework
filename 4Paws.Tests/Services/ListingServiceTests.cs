using Moq;
using Xunit;
using FluentAssertions;
using _4Paws.Services.Listing;
using _4Paws.DTOs.Listing.Requests;
using _4Paws.Data;
using _4Paws.Helper.Owner;
using _4Paws.Helper.CareGiver;
using _4Paws.Models;
using _4Paws.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace _4Paws.Tests.Services
{
    public class ListingServiceTests
    {
        private readonly Mock<DataContext> _mockDbContext;
        private readonly Mock<ICurrentOwner> _mockCurrentOwner;
        private readonly Mock<ICurrentCareGiver> _mockCurrentCareGiver;
        private readonly IMemoryCache _memoryCache;
        private readonly ListingService _listingService;

        public ListingServiceTests()
        {
            _mockDbContext = new Mock<DataContext>();
            _mockCurrentOwner = new Mock<ICurrentOwner>();
            _mockCurrentCareGiver = new Mock<ICurrentCareGiver>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _listingService = new ListingService(
                _mockDbContext.Object,
                _mockCurrentOwner.Object,
                _mockCurrentCareGiver.Object,
                _memoryCache);
        }

        #region CreateListing Tests

        [Fact]
        public void CreateListing_WithNullRequest_ReturnsBadRequest()
        {
            // Act
            var result = _listingService.CreateListing(null!);

            // Assert
            result.Status.Should().Be(400);
            result.Message.Should().Contain("Request is null");
        }

        [Fact]
        public void CreateListing_OwnerListingWithoutOwnerProfile_ReturnsNotFound()
        {
            // Arrange
            _mockCurrentOwner.Setup(x => x.GetCurrentOwner()).Returns((Owner)null!);

            var createListingRequest = new CreateListingRequest
            {
                ListingType = ListingType.OwnerNeedsCareGiver,
                Title = "Need caregiver",
                Description = "Looking for a caregiver",
                PetId = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                ProposedBudget = 100
            };

            // Act
            var result = _listingService.CreateListing(createListingRequest);

            // Assert
            result.Status.Should().Be(404);
            result.Message.Should().Contain("Owner profile not found");
        }

        [Fact]
        public void CreateListing_OwnerListingWithoutPetId_ReturnsBadRequest()
        {
            // Arrange
            var owner = new Owner { Id = 1, UserId = 1 };
            _mockCurrentOwner.Setup(x => x.GetCurrentOwner()).Returns(owner);

            var createListingRequest = new CreateListingRequest
            {
                ListingType = ListingType.OwnerNeedsCareGiver,
                Title = "Need caregiver",
                Description = "Looking for a caregiver",
                PetId = null,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                ProposedBudget = 100
            };

            // Act
            var result = _listingService.CreateListing(createListingRequest);

            // Assert
            result.Status.Should().Be(400);
            result.Message.Should().Contain("PetId is required");
        }

        [Fact]
        public void CreateListing_CareGiverListingWithoutProfile_ReturnsNotFound()
        {
            // Arrange
            _mockCurrentCareGiver.Setup(x => x.GetCurrentCareGiver()).Returns((CareGiver)null!);

            var createListingRequest = new CreateListingRequest
            {
                ListingType = ListingType.CareGiverOffersService,
                Title = "Offering care services",
                Description = "I offer pet care services",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                ProposedBudget = 500
            };

            // Act
            var result = _listingService.CreateListing(createListingRequest);

            // Assert
            result.Status.Should().Be(404);
            result.Message.Should().Contain("Caregiver profile not found");
        }

        #endregion
    }
}