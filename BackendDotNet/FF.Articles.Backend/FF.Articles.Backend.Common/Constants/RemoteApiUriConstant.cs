namespace FF.Articles.Backend.Common.Constants;
public static class RemoteApiUriConstant
{
    public const string IdentityBaseUri = "http://localhost:22000";
    public const string ContentsBaseUri = "http://localhost:23000";
    public const string AIBaseUri = "http://localhost:24000";
    /// <summary>
    /// identityApi/api/identity/admin/get-dto/{userId}
    /// </summary>
    public static string GetUserApiDtoById(int userId) => IdentityBaseUri + $"/api/identity/users/{userId}";
    public static string ArticleBatchUri() => ContentsBaseUri + $"/api/contents/articles/batch";
    public static string ArticleUri() => ContentsBaseUri + $"/api/contents/articles/";
    public static string ArticleBatchEditContentUri() => ContentsBaseUri + $"/api/contents/articles/content/batch";
    public static string TopicUri() => ContentsBaseUri + $"/api/contents/topics";
}

