using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories;

namespace FF.Articles.Backend.Contents.API.UnitOfWork;
public interface IContentsUnitOfWork : IUnitOfWork<ContentsDbContext>
{
    IArticleRepository ArticleRepository { get; }
    IArticleTagRepository ArticleTagRepository { get; }
    ITopicRepository TopicRepository { get; }
    ITagRepository TagRepository { get; }
}

