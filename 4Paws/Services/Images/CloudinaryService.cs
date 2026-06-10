using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace _4Paws.Services.Images
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            var account = new Account("dmhpukjp6", "424886892394378", "VVOwaZj6f7-m2iIhkHedX_Y15W4");
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "4paws"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }

    }
}
