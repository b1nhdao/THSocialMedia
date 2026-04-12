namespace THSocialMedia.Application.UsecaseHandlers.Users.Commands
{
    public class AccpetRelationShipCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
    }
}
