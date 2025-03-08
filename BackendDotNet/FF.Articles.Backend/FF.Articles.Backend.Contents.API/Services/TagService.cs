using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Services.Interfaces;

namespace FF.Articles.Backend.Contents.API.Services;
public class TagService(ContentsDbContext _context, ILogger<TagService> _logger, IMapper _mapper,
    IArticleTagService _articleTagService
)
: BaseService<Tag, ContentsDbContext>(_context, _logger), ITagService
{
    public override async Task<bool> DeleteAsync(int id)
    {
        var tag = await this.GetByIdAsync(id);
        if (tag == null)
            return false;
        await _articleTagService.RemoveTag(id);
        await this.DeleteAsync(id);
        return true;
    }
}
