using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Dtos;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Contents.API.RemoteServices.Interfaces;
using System.Net.Http;
using System.Security.Principal;
using System.Text.Json;

namespace FF.Articles.Backend.Contents.API.RemoteServices;
public class IdentityRemoteService(HttpClient _httpClient)
    : IIdentityRemoteService
{
    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var response = await _httpClient.GetAsync(RemoteApiUrlConstant.GetUserDtoById(userId));
        if (!response.IsSuccessStatusCode)
        {
            return null; 
        }

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
        return apiResponse?.Data;
    }
}
