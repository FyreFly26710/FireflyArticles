using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Repositories.Interfaces;
using FF.Articles.Backend.Contents.API.Services.Interfaces;
using FF.Articles.Backend.Contents.API.UnitOfWork;

namespace FF.Articles.Backend.Contents.API.Services;
public class TagService(
    ITagRepository _tagRepository,
    IArticleTagService _articleTagService,
    IContentsUnitOfWork _contentsUnitOfWork,
    ILogger<TagService> _logger
)
: BaseService<Tag, ContentsDbContext>(_tagRepository, _logger), ITagService
{
    public override async Task<bool> DeleteAsync(int id)
    {
        if (!await _tagRepository.ExistsAsync(id))
            return true;
        await _contentsUnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _articleTagService.DeleteAsync(id);
            await _tagRepository.DeleteAsync(id);
        });
        return true;
    }
}
