using MediatR;
using Microsoft.AspNetCore.Mvc;
using THSocialMedia.Api.Extensions.Models;
using THSocialMedia.Application.Services.StorageService;
using THSocialMedia.Application.UsecaseHandlers.Uploads.Commands;

namespace THSocialMedia.Api.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadFileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UploadFileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("multiple")]
        public async Task<IActionResult> UploadFiles(UploadFilesCommand command)
        {
            return (await _mediator.Send(command)).ToActionResult();
        }
    }
}
