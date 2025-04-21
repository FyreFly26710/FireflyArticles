namespace FF.Articles.Backend.Common.Constants;
public static class RemoteApiUrlConstant
{
    public const string IdentityBaseUrl = "https://localhost:22000";
    public const string ContentsBaseUrl = "https://localhost:23000";
    public const string AIBaseUrl = "https://localhost:24000";
    /// <summary>
    /// identityApi/api/identity/admin/get-dto/{userId}
    /// </summary>
    public static string GetUserApiDtoById(long userId) => $"/api/identity/users/{userId}";
    public static string ArticleUrl() => $"/api/contents/articles";
    //public static string ArticleBatchEditContentUrl() => ContentsBaseUrl + $"/api/contents/articles/content/batch";
    public static string TopicUrl() => $"/api/contents/topics";
}

