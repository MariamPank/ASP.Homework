using OfficeSpaceRent.DTOs.Requests;
using OfficeSpaceRent.DTOs.Responses;

namespace OfficeSpaceRent.Services.Office
{
    public interface IOfficeService
    {
        List<OfficeResponse> GetAll(
            int? floor,
            double? minArea,
            double? maxArea,
            decimal? minPrice,
            decimal? maxPrice,
            bool? isAvailable);

        OfficeResponse? GetById(int id);
        OfficeResponse Create(CreateOfficeRequest request);
        OfficeResponse? Update(int id, UpdateOfficeRequest request);
        bool Delete(int id);
    }
}
