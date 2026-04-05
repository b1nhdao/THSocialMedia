namespace THSocialMedia.Application.Services.AuthService;

public interface IIdentityService
{
    Guid GetUserIdentity();

    string GetUserName();

    string? GetToken();
}
