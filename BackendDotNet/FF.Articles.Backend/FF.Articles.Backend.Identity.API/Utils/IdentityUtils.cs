using FF.Articles.Backend.Common.Dtos;
using FF.Articles.Backend.Identity.API.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json;

namespace FF.Articles.Backend.Identity.API.Utils;

public class IdentityUtils
{
    /// <summary>
    /// User info is stored in cliams. Claims are stored client side in cookies.
    /// Other API endpoints can access user info by decrypting the cookie.
    /// </summary>
    public static async Task SignIn(User user, HttpContext httpContext)
    {
        // Remove password from user object
        user.UserPassword = string.Empty;
        var userDto = new UserDto()
        {
            UserId = user.Id,
            CreateTime = user.CreateTime,
            UserAccount = user.UserAccount,
            UserEmail = user.UserEmail,
            UserName = user.UserName,
            UserAvatar = user.UserAvatar,
            UserProfile = user.UserProfile,
            UserRole = user.UserRole
        };
        string userJson = JsonSerializer.Serialize(userDto);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName ?? "Guest"),
            new Claim(ClaimTypes.Role, user.UserRole),
            new Claim("user", userJson),
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });
    }

    //public static User GetUserFromHttpRequest(HttpRequest request) 
    //    => JsonSerializer.Deserialize<User>(request.HttpContext.User.FindFirst("user").Value);

    public static async Task SignOutUser(HttpRequest httpRequest)
        => await httpRequest.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

}