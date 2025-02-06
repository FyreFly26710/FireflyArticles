using System;

namespace FF.Articles.Backend.Identity.API.Constants;

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
        /// Banned user role
        /// </summary>
        public const string BAN_ROLE = "ban";

        // endregion
    }