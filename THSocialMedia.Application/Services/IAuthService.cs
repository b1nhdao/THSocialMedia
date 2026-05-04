namespace THSocialMedia.Application.Services;

public interface IAuthService
{
    Guid GetUserIdentity();

    string GetUserName();

    string? GetToken();
}
