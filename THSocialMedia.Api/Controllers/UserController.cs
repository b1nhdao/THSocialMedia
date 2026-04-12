using MediatR;
using Microsoft.AspNetCore.Mvc;
using THSocialMedia.Api.Extensions.Models;
using THSocialMedia.Application.UsecaseHandlers.Users.Commands;
using THSocialMedia.Application.UsecaseHandlers.Users.Queries;

namespace THSocialMedia.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Created(nameof(GetById), result.Value) : BadRequest(result.Errors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetUserByIdQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllUsersQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteUserCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound();
        }

        [HttpPost("AddFriend")]
        public async Task<IActionResult> AddFriend([FromBody] SendRelationshipUserCommand command)
        {
            return (await _mediator.Send(command)).ToActionResult();
        }

        [HttpPut("AddFriend")]
        public async Task<IActionResult> AcceptRelationShip([FromBody] UpdateRelationshipUserCommand command)
        {
            return (await _mediator.Send(command)).ToActionResult();
        }

        [HttpGet("AddFriend")]
        public async Task<IActionResult> GetAllRelationShip([FromQuery] GetAllRelationshipsQuery command)
        {
            return (await _mediator.Send(command)).ToActionResult();
        }
    }
}
