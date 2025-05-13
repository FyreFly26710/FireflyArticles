namespace FF.Articles.Backend.Contents.API.Services;
public class TagService(
    ITagRepository _tagRepository,
    IArticleTagRepository _articleTagRepository,
    IContentsUnitOfWork _contentsUnitOfWork,
    ILogger<TagService> _logger
) : BaseService<Tag, ContentsDbContext>(_tagRepository, _logger), ITagService
{
    public override async Task<bool> DeleteAsync(long id)
    {
        if (!await _tagRepository.ExistsAsync(id))
            return true;
        await _contentsUnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _articleTagRepository.DeleteByTagId(id);
            await _tagRepository.DeleteAsync(id);
        });
        return true;
    }
    public async Task<Tag?> GetTagByNameAsync(string tagName)
    {
        return await _tagRepository.GetQueryable()
            .FirstOrDefaultAsync(t => t.TagName == tagName);
    }
}
