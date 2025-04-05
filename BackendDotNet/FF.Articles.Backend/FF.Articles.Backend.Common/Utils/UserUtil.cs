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
        if (!request.HttpContext.User.Identity.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        var user = request.HttpContext.User.FindFirst("user");
        if (user == null)
        {
            // Check what claims are actually available for debugging
            var availableClaims = request.HttpContext.User.Claims.Select(c => c.Type).ToList();
            var claimsMessage = string.Join(", ", availableClaims);
            
            throw new UnauthorizedAccessException($"User claim not found in the authenticated user. Available claims: {claimsMessage}");
        }
        
        try
        {
            return JsonSerializer.Deserialize<UserApiDto>(user.Value);
        }
        catch (JsonException ex)
        {
            throw new UnauthorizedAccessException($"Failed to deserialize user claim: {ex.Message}");
        }
    }
    public static long GetUserId(HttpRequest request)
    {
        var user = GetUserFromHttpRequest(request);
        return user.UserId;
    }
}