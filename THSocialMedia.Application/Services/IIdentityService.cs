namespace THSocialMedia.Application.Services;

public interface IIdentityService
{
    Guid GetUserIdentity();

    string GetUserName();

    string? GetToken();
}
