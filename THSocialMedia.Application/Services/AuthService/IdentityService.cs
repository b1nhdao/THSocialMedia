using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using THSocialMedia.Domain.Abstractions.IRepositories;

namespace THSocialMedia.Application.Services.AuthService;

public class IdentityService(IHttpContextAccessor context) : IIdentityService
{
    public Guid GetUserIdentity()
    {
        try
        {
            var userId = context.HttpContext?.User?.FindFirst("UserGuid")?.Value;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("UserId claim not found or invalid.");
            }

            Guid.TryParse(userId, out var parsedGuid);
            if (parsedGuid == Guid.Empty)
            {
                throw new UnauthorizedAccessException("UserId claim not found or invalid.");
            }

            return parsedGuid;

        }
        catch (Exception e)
        {
            return Guid.Parse("00000000-0000-0000-0000-000000000001");
        }
    }


    public string GetUserName()
        => context.HttpContext?.User.Identity?.Name;


    public string? GetToken()
    {
        try
        {
            var authorizationHeader = context.HttpContext?.Request?.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authorizationHeader))
                return null;

            if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return authorizationHeader["Bearer ".Length..].Trim();

            return authorizationHeader.Trim();
        }
        catch
        {
            return null;
        }
    }
}
