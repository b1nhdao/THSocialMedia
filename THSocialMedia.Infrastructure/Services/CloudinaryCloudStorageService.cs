using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using THSocialMedia.Application.Services.StorageService;

namespace THSocialMedia.Infrastructure.Services
{
    public class CloudinaryCloudStorageService : IStorageService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryCloudStorageService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<ResponseImageModel> UploadFileAsync(Stream fileStream, string fileName)
        {
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                PublicId = $"{fileName.Replace(" ", "")}_{Guid.CreateVersion7()}",
                Overwrite = true,
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return new ResponseImageModel
            {
                FileName = fileName,
                FileUrl = uploadResult.Url.ToString()
            };
        }
        public Task<string> GetUrlFile(string fileName)
        {
            throw new NotImplementedException();
        }

    }
}
