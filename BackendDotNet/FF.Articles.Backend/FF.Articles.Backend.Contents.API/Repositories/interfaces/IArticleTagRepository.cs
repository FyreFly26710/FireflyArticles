using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.Repositories.Interfaces;
public interface IArticleTagRepository : IBaseRepository<ArticleTag, ContentsDbContext>
{
}


