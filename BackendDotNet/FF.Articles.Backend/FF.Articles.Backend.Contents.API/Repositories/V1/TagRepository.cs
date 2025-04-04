using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V1;
using FF.Articles.Backend.Contents.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Repositories.V1;
public class TagRepository : BaseRepository<Tag, ContentsDbContext>, ITagRepository
{
    public TagRepository(ContentsDbContext _context) : base(_context)
    {
    }
    public async Task<List<Tag>> GetByNamesAsync(List<string> names)
    {
        names = names.Select(n => n.ToLower().Trim()).Distinct().ToList();
        names = names.Where(n => !string.IsNullOrEmpty(n)).ToList();
        return await base.GetQueryable().Where(t => names.Contains(t.TagName.ToLower().Trim())).ToListAsync();
    }

    public async Task<List<Tag>> GetOrCreateByNamesAsync(List<string> names)
    {
        names = names.Select(n => n.ToLower().Trim())
                    .Distinct()
                    .Where(n => !string.IsNullOrEmpty(n.Trim()))
                    .ToList();

        var existingTags = await base.GetQueryable()
            .Where(t => names.Contains(t.TagName.ToLower().Trim()))
            .ToListAsync();

        var existingNames = existingTags.Select(t => t.TagName.ToLower().Trim()).ToList();
        var missingNames = names.Except(existingNames).ToList();

        if (missingNames.Any())
        {
            foreach (var name in missingNames)
            {
                var id = await CreateAsync(new Tag { TagName = name });
                existingTags.Add(new Tag { Id = id, TagName = name });
            }
        }

        return existingTags;
    }


}
