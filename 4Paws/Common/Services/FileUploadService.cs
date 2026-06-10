namespace _4Paws.Common.Services
{
    public class FileUploadService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public FileUploadService(IWebHostEnvironment env) => _env = env;

        public async Task<string> SaveImageAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file provided.");

            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!_allowedExtensions.Contains(ext))
                throw new Exception("Only JPG, PNG and WebP files are allowed.");

            if (file.Length > MaxFileSize)
                throw new Exception("File size must be under 5MB.");

            // Ensure wwwroot/uploads/{folder} exists
            var uploadPath = Path.Combine(_env.ContentRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadPath);

            // Use a GUID filename to prevent collisions and hide original name
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Return the relative URL — used to serve the file via UseStaticFiles
            return $"/uploads/{folder}/{fileName}";
        }

        public void DeleteImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            var fullPath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}

