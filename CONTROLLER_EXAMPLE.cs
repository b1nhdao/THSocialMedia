using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using THSocialMedia.Application.UsecaseHandlers.Posts.Commands;
using THSocialMedia.Application.UsecaseHandlers.Posts.Queries;
using THSocialMedia.Application.UsecaseHandlers.Posts.ViewModels;

namespace THSocialMedia.Api.Controllers
{
    /// <summary>
    /// Example controller showing CQRS pattern usage
    /// Commands write to PostgreSQL and trigger events
    /// Queries read from MongoDB optimized read models
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PostsController> _logger;

        public PostsController(IMediator mediator, ILogger<PostsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Create a new post
        /// Command: Writes to PostgreSQL ? Publishes PostCreatedEvent ? MongoDB synced
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<Result<Guid>>> CreatePost(CreatePostCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Post created successfully: {PostId}", result.Value);
                    return Ok(result);
                }

                return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post");
                return StatusCode(StatusCodes.Status500InternalServerError, Result.Error("Failed to create post"));
            }
        }

        /// <summary>
        /// Update an existing post
        /// Command: Writes to PostgreSQL ? Publishes PostUpdatedEvent ? MongoDB synced
        /// </summary>
        [HttpPut("update/{id:guid}")]
        public async Task<ActionResult<Result<Guid>>> UpdatePost(Guid id, UpdatePostCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Post updated successfully: {PostId}", id);
                    return Ok(result);
                }

                if (result.NotFound)
                    return NotFound(result);

                return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating post {PostId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, Result.Error("Failed to update post"));
            }
        }

        /// <summary>
        /// Delete a post
        /// Command: Removes from PostgreSQL ? Publishes PostDeletedEvent ? Removed from MongoDB
        /// </summary>
        [HttpDelete("delete/{id:guid}")]
        public async Task<ActionResult<Result<bool>>> DeletePost(Guid id)
        {
            try
            {
                var command = new DeletePostCommand { Id = id };
                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Post deleted successfully: {PostId}", id);
                    return Ok(result);
                }

                if (result.NotFound)
                    return NotFound(result);

                return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post {PostId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, Result.Error("Failed to delete post"));
            }
        }

        /// <summary>
        /// Get post by ID from MongoDB read model
        /// Query: Reads optimized data from MongoDB
        /// </summary>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<PostViewModel>>> GetPostById(Guid id)
        {
            try
            {
                var query = new GetPostByIdReadQuery(id);
                var result = await _mediator.Send(query);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Post retrieved successfully: {PostId}", id);
                    return Ok(result);
                }

                if (result.NotFound)
                    return NotFound(result);

                return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving post {PostId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, Result.Error("Failed to retrieve post"));
            }
        }

        /// <summary>
        /// Get all posts from MongoDB read model
        /// Query: Reads optimized data from MongoDB, sorted by creation date (newest first)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Result<IEnumerable<PostViewModel>>>> GetAllPosts()
        {
            try
            {
                var query = new GetAllPostsReadQuery();
                var result = await _mediator.Send(query);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Posts retrieved successfully");
                    return Ok(result);
                }

                return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving posts");
                return StatusCode(StatusCodes.Status500InternalServerError, Result.Error("Failed to retrieve posts"));
            }
        }
    }
}
