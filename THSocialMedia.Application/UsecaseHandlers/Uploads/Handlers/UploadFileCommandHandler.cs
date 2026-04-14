using THSocialMedia.Application.Services.StorageService;
using THSocialMedia.Application.UsecaseHandlers.Uploads.Commands;

namespace THSocialMedia.Application.UsecaseHandlers.Uploads.Handlers
{
    public class UploadFileCommandHandler : IRequestHandler<UploadFilesCommand, Result<List<ResponseImageModel>>>
    {
        private readonly IStorageService _cloudStorageService;

        public UploadFileCommandHandler(IStorageService cloudStorageService)
        {
            _cloudStorageService = cloudStorageService;
        }

        public async Task<Result<List<ResponseImageModel>>> Handle(UploadFilesCommand request, CancellationToken cancellationToken)
        {
            var uploadResults = new List<ResponseImageModel>();
            foreach (var file in request.Files)
            {
                var uploadResult = await _cloudStorageService.UploadFileAsync(file.OpenReadStream(), file.FileName);
                uploadResults.Add(uploadResult);
            }

            return Result<List<ResponseImageModel>>.Success(uploadResults);
        }
    }
}
