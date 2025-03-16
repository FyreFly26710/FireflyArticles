using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories;

namespace FF.Articles.Backend.Contents.API.UnitOfWork;
public class ContentsUnitOfWork(
    ContentsDbContext context,
    IArticleRepository _articleRepository,
    IArticleTagRepository _articleTagRepository,
    ITopicRepository _topicRepository,
    ITagRepository _tagRepository
    )
: UnitOfWork<ContentsDbContext>(context), IContentsUnitOfWork
{
    public IArticleRepository ArticleRepository { get; } = _articleRepository;
    public IArticleTagRepository ArticleTagRepository { get; } = _articleTagRepository;
    public ITopicRepository TopicRepository { get; } = _topicRepository;
    public ITagRepository TagRepository { get; } = _tagRepository;
}
