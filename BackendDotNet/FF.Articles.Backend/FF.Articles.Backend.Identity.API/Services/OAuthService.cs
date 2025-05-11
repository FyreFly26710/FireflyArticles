using System.Net.Http.Headers;
using System.Text.Json;

namespace FF.Articles.Backend.Identity.API.Services;

public class OAuthService(IConfiguration _configuration, ILogger<OAuthService> _logger) : IOAuthService
{
    public async Task<TokenResponse> GetGmailToken(string code)
    {
        var tokenEndpoint = "https://oauth2.googleapis.com/token";
        string clientId = _configuration["GmailOAuth:ClientId"] ?? "";
        string clientSecret = _configuration["GmailOAuth:ClientSecret"] ?? "";
        string baseUrl = _configuration["Domain:Api"]?.TrimEnd('/') ?? "https://localhost:21000";
        string redirectUri = $"{baseUrl}/api/identity/auth/signin-google";

        var requestBody = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", clientId},
            { "client_secret", clientSecret},
            { "redirect_uri", redirectUri},
            { "grant_type", "authorization_code" }
        };

        using var httpClient = new HttpClient();
        var content = new FormUrlEncodedContent(requestBody);

        var response = await httpClient.PostAsync(tokenEndpoint, content);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get token from Google. Status code: {StatusCode}, Response: {Response}",
                response.StatusCode, responseContent);
        }

        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

        return tokenResponse ?? new TokenResponse();
    }

    public async Task<UserInfo> GetUserInfoFromGmailToken(string gmailToken)
    {
        if (string.IsNullOrEmpty(gmailToken))
            return new UserInfo();

        var userInfoEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", gmailToken);

        var response = await httpClient.GetAsync(userInfoEndpoint);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get user info from Google. Status code: {StatusCode}, Response: {Response}",
                response.StatusCode, responseContent);
        }

        var userInfo = JsonSerializer.Deserialize<UserInfo>(responseContent);
        return userInfo ?? new UserInfo();
    }
}
