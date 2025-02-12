using FF.Articles.Backend.Common.Dtos;

namespace FF.Articles.Backend.Contents.API.RemoteServices.Interfaces;
public interface IIdentityRemoteService
{
    Task<UserDto?> GetUserByIdAsync(int userId);

}
