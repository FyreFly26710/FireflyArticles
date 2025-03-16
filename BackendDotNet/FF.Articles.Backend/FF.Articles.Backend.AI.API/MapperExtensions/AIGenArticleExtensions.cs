using System;
using FF.Articles.Backend.AI.API.Models.AiDtos;
using FF.Articles.Backend.Common.ApiDtos;

namespace FF.Articles.Backend.AI.API.MapperExtensions;

public static class AIGenArticleExtensions
{
    public static ArticleApiAddRequest ToArticleApiRequest(this AIGenArticle aiGenArticle, int topicId)
    {
        return new ArticleApiAddRequest
        {
            Title = aiGenArticle.Title,
            // Content = aiGenArticle.Content ?? "",
            Abstraction = aiGenArticle.Abstraction,
            ArticleType = "Article",
            ParentArticleId = null,
            TopicId = topicId,
            TagIds = new List<int>(),
            SortNumber = aiGenArticle.Id,
            IsHidden = 0,
        };
    }
}
