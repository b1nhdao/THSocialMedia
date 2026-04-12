using MediatR;
using Microsoft.AspNetCore.Mvc;
using THSocialMedia.Api.Extensions.Models;
using THSocialMedia.Application.UsecaseHandlers.Posts.Commands;
using THSocialMedia.Application.UsecaseHandlers.Posts.Queries;

namespace THSocialMedia.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePostCommand command)
        {
            return (await _mediator.Send(command)).ToActionResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return (await _mediator.Send(new GetPostByIdReadQuery { Id = id })).ToActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return (await _mediator.Send(new GetAllPostsReadQuery())).ToActionResult();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePostCommand command)
        {
            return (await _mediator.Send(command)).ToActionResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return (await _mediator.Send(new DeletePostCommand { Id = id })).ToActionResult();
        }

        [HttpPost("{id}/comment")]
        public async Task<IActionResult> CommentPost(Guid id, CommentPostCommand command)
        {
            command.PostId = id;
            return (await _mediator.Send(command)).ToActionResult();
        }

        [HttpPost("{id}/react")]
        public async Task<IActionResult> ReactPost(Guid id, AddReactionPostCommand command)
        {
            command.PostId = id;
            return (await _mediator.Send(command)).ToActionResult();
        }
    }
}
