using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Constants;
public static class RemoteApiUrlConstant
{
    public const string IdentityBaseUrl = "http://localhost:22000";
    /// <summary>
    /// identityApi/api/identity/admin/get-dto/{userId}
    /// </summary>
    public static string GetUserDtoById(int userId) => IdentityBaseUrl + $"/api/identity/admin/get-dto/{userId}";
}

