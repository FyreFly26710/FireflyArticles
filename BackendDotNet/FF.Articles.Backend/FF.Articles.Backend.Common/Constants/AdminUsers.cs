using FF.Articles.Backend.Common.ApiDtos;

namespace FF.Articles.Backend.Common.Constants;

public static class AdminUsers
{
    public static readonly UserApiDto SYSTEM_ADMIN_FIREFLY = new UserApiDto
    {
        UserId = 1,
        UserName = "Firefly Admin",
        UserRole = UserConstant.ADMIN_ROLE,
        UserAccount = "firefly",
        UserAvatar = "https://i.imgur.com/64GJYtY.png",
        UserProfile = "",
        UserEmail = "lee.wan1204@gmail.com",
    };
    public static readonly UserApiDto SYSTEM_ADMIN_DEEPSEEK = new UserApiDto
    {
        UserId = 10,
        UserName = "DeepSeek Assistant",
        UserRole = UserConstant.ADMIN_ROLE,
        UserAccount = "deepseek",
        UserAvatar = "https://i.imgur.com/f6X5kDM.jpg",
        // UserProfile = "",
        // UserEmail = "",
    };
    public static readonly UserApiDto SYSTEM_ADMIN_GEMINI = new UserApiDto
    {
        UserId = 11,
        UserName = "Gemini Assistant",
        UserRole = UserConstant.ADMIN_ROLE,
        UserAccount = "gemini",
        UserAvatar = "https://i.imgur.com/o7ujFyz.png",
        // UserProfile = "",
        // UserEmail = "",
    };
}

