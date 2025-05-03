using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;

namespace FF.Articles.Backend.Contents.API.MapperExtensions;
public static class ArticleExtensions
{
    public static ArticleDto ToDto(this Article article)
    {
        var articleDto = new ArticleDto();
        articleDto.ArticleId = article.Id;
        articleDto.Title = article.Title;
        articleDto.Content = article.Content;
        articleDto.Abstract = article.Abstract;
        articleDto.ArticleType = article.ArticleType;
        articleDto.ParentArticleId = article.ParentArticleId;
        articleDto.TopicId = article.TopicId;
        articleDto.SortNumber = article.SortNumber;
        articleDto.IsHidden = article.IsHidden;
        articleDto.CreateTime = article.CreateTime;
        articleDto.UpdateTime = article.UpdateTime;
        articleDto.UserId = article.UserId;
        return articleDto;
    }
    public static Article ToEntity(this ArticleAddRequest articleAddRequest, long? userId = null)
    {
        var article = new Article();
        if (userId != null)
        {
            article.UserId = userId.Value;
        }
        article.Id = articleAddRequest.Id ?? EntityUtil.GenerateSnowflakeId();
        article.Title = articleAddRequest.Title;
        article.Content = articleAddRequest.Content;
        article.Abstract = articleAddRequest.Abstract;
        article.ArticleType = articleAddRequest.ArticleType;
        article.ParentArticleId = articleAddRequest.ParentArticleId;
        article.TopicId = articleAddRequest.TopicId;
        article.SortNumber = articleAddRequest.SortNumber;
        article.IsHidden = articleAddRequest.IsHidden;
        return article;
    }
}