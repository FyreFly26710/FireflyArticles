using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Contents.API.Interfaces.Services.RemoteServices;

namespace FF.Articles.Backend.Contents.API.Services.RemoteServices;
public class IdentityRemoteService(HttpClient _httpClient)
    : IIdentityRemoteService
{
    public async Task<UserApiDto?> GetUserByIdAsync(int userId)
    {
        var response = await _httpClient.GetAsync(RemoteApiUriConstant.GetUserApiDtoById(userId));
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserApiDto>>();
        return apiResponse?.Data;
    }
}
