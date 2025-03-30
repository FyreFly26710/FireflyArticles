using System;
using FF.Articles.Backend.AI.API.Models.AiDtos;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Utils;

namespace FF.Articles.Backend.AI.API.MapperExtensions;

public static class AIGenArticleExtensions
{
    public static ArticleApiAddRequest ToArticleApiRequest(this AIGenArticle aiGenArticle, long topicId)
    {
        return new ArticleApiAddRequest
        {
            Id = EntityUtil.GenerateSnowflakeId(),
            Title = aiGenArticle.Title,
            //Content = aiGenArticle.Content ?? "",
            Abstract = aiGenArticle.Abstract,
            ArticleType = "Article",
            ParentArticleId = null,
            TopicId = topicId,
            Tags = aiGenArticle.Tags,
            SortNumber = aiGenArticle.Id,
            IsHidden = 0,
        };
    }
}
