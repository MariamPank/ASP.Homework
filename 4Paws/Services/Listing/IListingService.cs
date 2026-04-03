using _4Paws.Common.Results;
using _4Paws.DTOs.Listing.Requests;
using _4Paws.DTOs.Listing.Responses;

namespace _4Paws.Services.Listing
{
    public interface IListingService
    {
        Result<ListingResponse> CreateListing(CreateListingRequest request);
        Result<ListingResponse> GetListingById(int id);
        Result<IEnumerable<ListingResponse>> GetAllActiveListings();
        Result<IEnumerable<ListingResponse>> GetMyListings();
        Result<ListingResponse> UpdateListing(int id, UpdateListingRequest request);
        Result<bool> DeleteListing(int id);
    }
}
