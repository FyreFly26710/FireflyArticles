using FF.Articles.Backend.Common.ApiDtos;

namespace FF.Articles.Backend.Contents.API.RemoteServices.Interfaces;
public interface IIdentityRemoteService
{
    Task<UserApiDto?> GetUserByIdAsync(int userId);

}
