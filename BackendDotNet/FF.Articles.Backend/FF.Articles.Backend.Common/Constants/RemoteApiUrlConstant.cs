using System.Web;

namespace FF.Articles.Backend.Common.Constants;
public static class RemoteApiUrlConstant
{
    /// <summary>
    /// Only run production in docker
    /// </summary>
    private static string GetBasePort() => EnvUtil.IsProduction() ? "http://127.0.0.1:" : "https://localhost:";

    public static string GetIdentityBaseUrl() => $"{GetBasePort()}22000";
    public static string GetContentsBaseUrl() => $"{GetBasePort()}23000";
    public static string GetAIBaseUrl() => $"{GetBasePort()}24000";
    public static string GetUserApiDtoById(long userId) => $"/api/identity/users/{userId}";
    public static string ArticleUrl() => $"/api/contents/articles";
    public static string ArticleUrl(long articleId) => $"/api/contents/articles/{articleId}";
    public static string TopicUrl() => $"/api/contents/topics";
    public static string TopicUrl(long topicId) => $"/api/contents/topics/{topicId}";
    public static string TopicUrlByTitleCategory(string title, string category) =>
        $"/api/contents/topics/search?Title={HttpUtility.UrlEncode(title)}&Category={HttpUtility.UrlEncode(category)}";
}

