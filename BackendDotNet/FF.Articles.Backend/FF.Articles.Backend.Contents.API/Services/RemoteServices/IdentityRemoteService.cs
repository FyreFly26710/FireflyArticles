namespace FF.Articles.Backend.Contents.API.Services.RemoteServices;
public class IdentityRemoteService(
    HttpClient _httpClient,
    ILogger<IdentityRemoteService> _logger
) : IIdentityRemoteService
{
    public async Task<UserApiDto?> GetUserByIdAsync(long userId)
    {
        var baseUrl = RemoteApiUrlConstant.GetIdentityBaseUrl();
        var url = baseUrl + RemoteApiUrlConstant.GetUserApiDtoById(userId);
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to get user by id");
        }
        UserApiDto userApiDto = new();
        try
        {
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserApiDto>>();
            userApiDto = apiResponse?.Data ?? throw new ApiException(ErrorCode.SYSTEM_ERROR);
        }
        catch (Exception ex)
        {
            userApiDto = new UserApiDto
            {
                UserId = userId,
                UserName = "Unknown",
                UserAvatar = "https://via.placeholder.com/150"
            };
            _logger.LogError(ex, "Failed to get user by id");
        }
        return userApiDto;
    }
}
