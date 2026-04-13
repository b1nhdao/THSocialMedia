using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace THSocialMedia.Application.Commons.Jwt
{
    public sealed class JwtTokenProvider : IJwtTokenProvider
    {
        private readonly JwtOptions _options;

        public JwtTokenProvider(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public string CreateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
