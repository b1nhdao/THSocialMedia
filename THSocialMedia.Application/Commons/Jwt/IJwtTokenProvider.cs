using System.Security.Claims;

namespace THSocialMedia.Application.Commons.Jwt
{
    public interface IJwtTokenProvider
    {
        string CreateAccessToken(IEnumerable<Claim> claims);
    }
}
