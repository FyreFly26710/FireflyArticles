using System;
using FF.Articles.Backend.Identity.API.Models.OAuth;

namespace FF.Articles.Backend.Identity.API.Services;

public interface IOAuthService
{
    public Task<TokenResponse> GetGmailToken(string code);
    public Task<UserInfo> GetUserInfoFromGmailToken(string gmailToken);
}
