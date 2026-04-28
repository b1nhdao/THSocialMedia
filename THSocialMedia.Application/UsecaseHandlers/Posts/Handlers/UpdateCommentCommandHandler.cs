using Ardalis.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using THSocialMedia.Application.Services;
using THSocialMedia.Application.UsecaseHandlers.Posts.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Events;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Result<Guid>>
{
    private readonly IPostWriteRepository _postRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;
    private readonly ILogger<UpdateCommentCommandHandler> _logger;

    public UpdateCommentCommandHandler(
        IPostWriteRepository postRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        ILogger<UpdateCommentCommandHandler> logger)
    {
        _postRepository = postRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetFirstOrDefault(
            x => x.Id == request.PostId,
            include: q => q.Include(x => x.Comments));

        if (post == null)
            return Result<Guid>.NotFound();

        var existingComment = post.Comments.FirstOrDefault(c => c.Id == request.CommentId);
        if (existingComment == null)
            return Result<Guid>.NotFound();

        var userId = _identityService.GetUserIdentity();
        if (existingComment.UserId != userId)
            return Result<Guid>.Forbidden();

        existingComment.Content = request.Content;
        existingComment.FileUrl = request.FileUrl;

        post.UpdateComment(existingComment);

        _postRepository.Update(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish event to sync to MongoDB
        try
        {
            var @event = new CommentUpdatedEvent(request.PostId, request.CommentId, userId, request.Content, request.FileUrl);
            await _eventBus.PublishEventAsync(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish CommentUpdatedEvent for post {PostId}", request.PostId);
        }

        return Result<Guid>.Success(existingComment.Id);
    }
}
