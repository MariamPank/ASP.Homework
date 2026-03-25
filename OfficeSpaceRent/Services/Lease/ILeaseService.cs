using OfficeSpaceRent.DTOs.Responses;

namespace OfficeSpaceRent.Services.Lease
{
    public interface ILeaseService
    {
        List<LeaseResponse> GetMyLeases(int userId);
        List<LeaseResponse> GetAll();
    }
}
