namespace THSocialMedia.Application.Services.StorageService
{
    public interface IStorageService
    {
        public Task<ResponseImageModel> UploadFileAsync(Stream fileStream, string fileName);
        public Task<string> GetUrlFile(string fileName);
    }
}
