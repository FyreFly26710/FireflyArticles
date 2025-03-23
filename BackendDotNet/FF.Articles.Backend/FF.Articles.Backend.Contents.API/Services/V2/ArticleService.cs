using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;

namespace FF.Articles.Backend.Contents.API.Services.V2;
public class ArticleService : RedisService<Article>, IArticleService
{
    public ArticleService(IArticleRedisRepository articleRedisRepository, ILogger<ArticleService> logger)
        : base(articleRedisRepository, logger)
    {
    }

    public Task<Dictionary<int, string>> CreateBatchAsync(List<ArticleAddRequest> articleAddRequests, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<int> CreateByRequest(ArticleAddRequest articleAddRequest, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteArticleById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EditArticleByRequest(ArticleEditRequest articleEditRequest)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EditContentBatch(Dictionary<int, string> batchEditConentRequests)
    {
        throw new NotImplementedException();
    }

    public Task<ArticleDto> GetArticleDto(Article article)
    {
        throw new NotImplementedException();
    }

    public Task<ArticleDto> GetArticleDto(Article article, ArticleQueryRequest articleRequest)
    {
        throw new NotImplementedException();
    }

    public Task<List<ArticleDto>> GetArticleDtos(IEnumerable<Article> articles, ArticleQueryRequest articleRequest)
    {
        throw new NotImplementedException();
    }

    public Task<Paged<ArticleDto>> GetArticlesByPageRequest(ArticleQueryRequest pageRequest)
    {
        throw new NotImplementedException();
    }
}

