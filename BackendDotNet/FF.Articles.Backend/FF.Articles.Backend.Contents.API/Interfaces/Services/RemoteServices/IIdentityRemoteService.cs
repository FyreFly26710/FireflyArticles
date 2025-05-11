namespace FF.Articles.Backend.Contents.API.Interfaces.Services.RemoteServices;
public interface IIdentityRemoteService
{
    Task<UserApiDto?> GetUserByIdAsync(long userId);

}
