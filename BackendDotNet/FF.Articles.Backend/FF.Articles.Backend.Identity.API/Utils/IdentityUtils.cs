using FF.Articles.Backend.Common.Dtos;
using FF.Articles.Backend.Identity.API.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace FF.Articles.Backend.Identity.API.Utils;

public class IdentityUtils
{
    public static async Task SignInUser(User user, HttpContext httpContext)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserAccount),
            new Claim(ClaimTypes.Role, user.UserRole),
            new Claim("Id", user.Id.ToString()),
            new Claim("UserName", user.UserName ?? string.Empty),
            new Claim("UserAvatar", user.UserAvatar ?? string.Empty),
            new Claim("UserProfile", user.UserProfile ?? string.Empty),
            new Claim("CreateTime", user.CreateTime.ToString() ?? string.Empty)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
    }

    public static User GetUserFromHttpRequest(HttpRequest request)
    {
        var claimsPrincipal = request.HttpContext.User;
        if (claimsPrincipal == null)
        {
            return null;
        }
        var userAccount = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        var userRole = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;
        var idString = claimsPrincipal.FindFirst("Id")?.Value;
        var userName = claimsPrincipal.FindFirst("UserName")?.Value;
        var userAvatar = claimsPrincipal.FindFirst("UserAvatar")?.Value;
        var userProfile = claimsPrincipal.FindFirst("UserProfile")?.Value;
        var createTimeString = claimsPrincipal.FindFirst("CreateTime")?.Value;

        if (userAccount != null && userRole != null && idString != null)
        {
            int id = int.TryParse(idString, out var parsedId) ? parsedId : 0;
            DateTime createTime = DateTime.Parse(createTimeString) ;

            return new User
            {
                Id = id,
                UserAccount = userAccount,
                UserRole = userRole,
                UserName = userName,
                UserAvatar = userAvatar,
                UserProfile = userProfile,
                CreateTime = createTime
            };
        }

        return null;
    }
    public static async Task<bool> SignOutUser(HttpRequest httpRequest)
    {
        await httpRequest.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return true;
    }
}