using FF.Articles.Backend.Common.ApiDtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Utils;

public class UserUtil
{
    public static UserApiDto GetUserFromHttpRequest(HttpRequest request)
    {
        var user = request.HttpContext.User.FindFirst("user");
        if (user == null)
            throw new UnauthorizedAccessException("User not found");
        return JsonSerializer.Deserialize<UserApiDto>(user.Value);
    }
    public static int GetUserId(HttpRequest request)
    {
        var user = GetUserFromHttpRequest(request);
        return user.UserId;
    }
}