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
        names = names.Select(n => n.Normalize()).Distinct().ToList();
        names = names.Where(n => !string.IsNullOrEmpty(n)).ToList();
        return await base.GetQueryable().Where(t => names.Contains(t.TagName.Normalize())).ToListAsync();
    }

    public async Task<List<Tag>> GetOrCreateByNamesAsync(List<string> names)
    {
        // Normalize and deduplicate the names
        names = names.Select(n => n.Normalize())
                    .Distinct()
                    .Where(n => !string.IsNullOrEmpty(n))
                    .ToList();

        if (names.Count == 0) return [];


        // Get all existing tags in one query with case-insensitive comparison
        // Db may have duplicates, so we need to handle them by taking the first tag for each name
        var allExistingTags = await base.GetQueryable()
            .Where(t => names.Contains(t.TagName.Normalize()))
            .ToListAsync();

        // Handle potential duplicates in the database by taking the first tag for each name
        var existingTags = new List<Tag>();
        var processedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tag in allExistingTags)
        {
            var normalizedName = tag.TagName.Normalize();
            if (!processedNames.Contains(normalizedName))
            {
                existingTags.Add(tag);
                processedNames.Add(normalizedName);
            }
        }

        var tagsToCreate = names
            .Where(name => !existingTags.Any(t => t.TagName.Normalize() == name.Normalize()))
            .ToList();

        // Create new tags if needed
        foreach (var name in tagsToCreate)
        {
            var tag = new Tag { TagName = name };
            var id = await CreateAsync(tag);
            tag.Id = id;
            existingTags.Add(tag);
        }

        return existingTags;
    }
}
