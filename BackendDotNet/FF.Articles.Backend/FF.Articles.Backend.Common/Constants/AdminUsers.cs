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
        UserAvatar = "https://media.tenor.com/73ZvmoXxc-IAAAAe/firefly-honkai-star-rail.png",
        UserProfile = "",
        UserEmail = "lee.wan1204@gmail.com",
    };
    public static readonly UserApiDto SYSTEM_ADMIN_DEEPSEEK = new UserApiDto
    {
        UserId = 10,
        UserName = "DeepSeek Assistant",
        UserRole = UserConstant.ADMIN_ROLE,
        UserAccount = "deepseek",
        UserAvatar = "https://www-cdn.morphcast.com/wp-content/uploads/2025/01/deepseek.jpg.webp",
        // UserProfile = "",
        // UserEmail = "",
    };
}

