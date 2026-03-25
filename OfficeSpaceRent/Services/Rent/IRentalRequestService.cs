using OfficeSpaceRent.DTOs.Requests;
using OfficeSpaceRent.DTOs.Responses;

namespace OfficeSpaceRent.Services.Rent
{
    public interface IRentalRequestService
    {
        RentalRequestResponse Create(int userId, CreateRentalRequest request);
        List<RentalRequestResponse> GetMyRequests(int userId);
        List<RentalRequestResponse> GetAll();
        RentalRequestResponse Approve(int requestId);
        RentalRequestResponse Reject(int requestId);
    }
}
