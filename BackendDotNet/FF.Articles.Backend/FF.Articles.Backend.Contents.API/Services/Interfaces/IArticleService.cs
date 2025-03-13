using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;

namespace FF.Articles.Backend.Contents.API.Services.Interfaces;
public interface IArticleService : IBaseService<Article, ContentsDbContext>
{
    Task<ArticleDto> GetArticleDto(Article article);
    Task<ArticleDto> GetArticleDto(Article article, ArticleQueryRequest articleRequest);
    Task<List<ArticleDto>> GetArticleDtos(IEnumerable<Article> articles, ArticleQueryRequest articleRequest);
    Task<bool> EditArticleByRequest(ArticleEditRequest articleEditRequest);
    Task<bool> DeleteArticleById(int id);
    Task<Paged<ArticleDto>> GetArticlesByPageRequest(ArticleQueryRequest pageRequest);
}
