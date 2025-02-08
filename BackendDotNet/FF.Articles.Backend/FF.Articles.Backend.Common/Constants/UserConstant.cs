using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Constants;
public static class UserConstant
{
    public const string USER_LOGIN_STATE = "user_login";

    // region Auth

    /// <summary>
    /// Default user role
    /// </summary>
    public const string DEFAULT_ROLE = "user";

    /// <summary>
    /// Admin user role
    /// </summary>
    public const string ADMIN_ROLE = "admin";

    /// <summary>
    /// Developer role 
    /// </summary>
    public const string DEVELOPER_ROLE = "dev";

    /// <summary>
    /// Banned user role
    /// </summary>
    public const string BAN_ROLE = "ban";

    // endregion
}