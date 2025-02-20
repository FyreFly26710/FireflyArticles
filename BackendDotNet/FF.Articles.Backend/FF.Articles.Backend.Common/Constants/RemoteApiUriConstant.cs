using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Constants;
public static class RemoteApiUriConstant
{
    public const string IdentityBaseUri = "http://localhost:22000";
    /// <summary>
    /// identityApi/api/identity/admin/get-dto/{userId}
    /// </summary>
    public static string GetUserApiDtoById(int userId) => IdentityBaseUri + $"/api/identity/users/{userId}";
}

