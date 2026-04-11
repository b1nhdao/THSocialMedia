using System.Security.Claims;

namespace THSocialMedia.Infrastructure.Services.Jwt
{
    public interface IJwtTokenProvider
    {
        string CreateAccessToken(IEnumerable<Claim> claims);
    }
}
