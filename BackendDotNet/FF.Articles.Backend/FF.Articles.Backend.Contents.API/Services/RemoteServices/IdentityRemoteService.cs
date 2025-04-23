using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Contents.API.Interfaces.Services.RemoteServices;

namespace FF.Articles.Backend.Contents.API.Services.RemoteServices;
public class IdentityRemoteService : IIdentityRemoteService
{
    private readonly HttpClient _httpClient;

    public IdentityRemoteService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserApiDto?> GetUserByIdAsync(long userId)
    {
        var baseUrl = RemoteApiUrlConstant.GetIdentityBaseUrl();
        var url = baseUrl + RemoteApiUrlConstant.GetUserApiDtoById(userId);
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to get user by id");
        }

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserApiDto>>();
        return apiResponse?.Data;
    }
}
