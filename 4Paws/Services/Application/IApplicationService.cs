using _4Paws.Common.Results;
using _4Paws.DTOs.Application.Requests;
using _4Paws.DTOs.Application.Responses;
using _4Paws.Enums;

namespace _4Paws.Services.Application
{
    public interface IApplicationService
    {
        Result<ApplicationResponse> ApplyToListing(ApplyRequest request);

        Result<IEnumerable<ApplicationResponse>> GetApplicationsForListing(int listingId);

        Result<IEnumerable<ApplicationResponse>> GetMyApplications();

        Result<ApplicationResponse> UpdateApplicationStatus(int applicationId, ApplicationStatus status);
    }
}
