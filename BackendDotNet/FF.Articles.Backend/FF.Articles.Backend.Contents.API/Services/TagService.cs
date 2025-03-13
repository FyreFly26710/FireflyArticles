using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Repositories.Interfaces;
using FF.Articles.Backend.Contents.API.Services.Interfaces;

namespace FF.Articles.Backend.Contents.API.Services;
public class TagService(ITagRepository _tagRepository, ILogger<TagService> _logger,
    IArticleTagService _articleTagService
)
: BaseService<Tag, ContentsDbContext>(_tagRepository, _logger), ITagService
{
    public override async Task<bool> DeleteAsync(int id)
    {
        if (await _tagRepository.ExistsAsync(id))
        {
            await _articleTagService.DeleteAsync(id);
            await _tagRepository.DeleteAsync(id);
        }
        return true;
    }
}
