using OfficeSpaceRent.DTOs.Requests;
using OfficeSpaceRent.DTOs.Responses;

namespace OfficeSpaceRent.Services.Rent
{
    public class RentalRequestService : IRentalRequestService
    {
        public RentalRequestResponse Approve(int requestId)
        {
            throw new NotImplementedException();
        }

        public RentalRequestResponse Create(int userId, CreateRentalRequest request)
        {
            throw new NotImplementedException();
        }

        public List<RentalRequestResponse> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<RentalRequestResponse> GetMyRequests(int userId)
        {
            throw new NotImplementedException();
        }

        public RentalRequestResponse Reject(int requestId)
        {
            throw new NotImplementedException();
        }
    }
}
