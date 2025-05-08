using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Exceptions;
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
        // if (!request.HttpContext.User.Identity.IsAuthenticated)
        // {
        //     throw new ApiException(ErrorCode.NOT_LOGIN_ERROR, "User is not authenticated");
        // }

        var user = request.HttpContext.User.FindFirst("user");
        if (user == null)
        {
            // Check what claims are actually available for debugging
            var availableClaims = request.HttpContext.User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            var claimsMessage = string.Join(", ", availableClaims);

            throw new ApiException(ErrorCode.NOT_LOGIN_ERROR, $"User claim not found in the authenticated user.");
        }

        try
        {
            return JsonSerializer.Deserialize<UserApiDto>(user.Value);
        }
        catch (JsonException ex)
        {
            throw new ApiException(ErrorCode.NOT_LOGIN_ERROR, $"Failed to deserialize user claim: {ex.Message}");
        }
    }

    /// <summary>
    /// Attempts to get user from the HTTP request, but doesn't throw if not found
    /// </summary>
    public static bool TryGetUserFromHttpRequest(HttpRequest request, out UserApiDto userDto)
    {
        userDto = null;

        try
        {
            userDto = GetUserFromHttpRequest(request);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static long GetUserId(HttpRequest request)
    {
        var user = GetUserFromHttpRequest(request);
        return user.UserId;
    }
}