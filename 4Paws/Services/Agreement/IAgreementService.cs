using _4Paws.Common.Results;
using _4Paws.DTOs.Agreement.Responses;

namespace _4Paws.Services.Agreement
{
    public interface IAgreementService
    {
        Result<AgreementResponse> CreateAgreement(int applicationId);
        Result<AgreementResponse> GetAgreementById(int id);
        Result<IEnumerable<AgreementResponse>> GetMyAgreements();
        Result<AgreementResponse> CompleteAgreement(int id);

    }
}
