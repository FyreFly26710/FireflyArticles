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
        => JsonSerializer.Deserialize<UserApiDto>(request.HttpContext.User.FindFirst("user").Value);
}