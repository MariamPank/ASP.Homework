using OfficeSpaceRent.DTOs.Requests;
using OfficeSpaceRent.DTOs.Responses;

namespace OfficeSpaceRent.Services.Office
{
    public class OfficeService : IOfficeService
    {
        public OfficeResponse Create(CreateOfficeRequest request)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<OfficeResponse> GetAll(int? floor, double? minArea, double? maxArea, decimal? minPrice, decimal? maxPrice, bool? isAvailable)
        {
            throw new NotImplementedException();
        }

        public OfficeResponse? GetById(int id)
        {
            throw new NotImplementedException();
        }

        public OfficeResponse? Update(int id, UpdateOfficeRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
