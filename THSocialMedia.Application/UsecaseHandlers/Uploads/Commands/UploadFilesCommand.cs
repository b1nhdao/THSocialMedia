using Microsoft.AspNetCore.Http;
using THSocialMedia.Application.Services.StorageService;

namespace THSocialMedia.Application.UsecaseHandlers.Uploads.Commands
{
    public class UploadFilesCommand : IRequest<Result<List<ResponseImageModel>>>
    {
        public List<IFormFile> Files { get; set; } = [];
    }
}
