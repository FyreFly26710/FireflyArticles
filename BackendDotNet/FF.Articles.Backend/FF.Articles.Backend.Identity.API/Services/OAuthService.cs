using System;
using System.Net.Http.Headers;
using System.Text.Json;
using FF.Articles.Backend.Identity.API.Models.OAuth;

namespace FF.Articles.Backend.Identity.API.Services;

public class OAuthService(IConfiguration _configuration) : IOAuthService
{
    public async Task<TokenResponse> GetGmailToken(string code)
    {
        var tokenEndpoint = "https://oauth2.googleapis.com/token";
        string clientId = _configuration["GmailOAuth:ClientId"] ?? "";
        string clientSecret = _configuration["GmailOAuth:ClientSecret"] ?? "";
        string RedirectUri = _configuration["Domain:Api"] ?? "" + "/api/identity/auth/signin-google";
        var requestBody = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", clientId},
            { "client_secret", clientSecret},
            { "redirect_uri", RedirectUri},
            { "grant_type", "authorization_code" }
        };

        using var httpClient = new HttpClient();
        var content = new FormUrlEncodedContent(requestBody);
        var response = await httpClient.PostAsync(tokenEndpoint, content);

        //response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

        return tokenResponse;
    }

    public async Task<UserInfo> GetUserInfoFromGmailToken(string gmailToken)
    {
        var userInfoEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", gmailToken);
        var response = await httpClient.GetAsync(userInfoEndpoint);

        //response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<UserInfo>(responseContent);

        return userInfo;
    }
}
