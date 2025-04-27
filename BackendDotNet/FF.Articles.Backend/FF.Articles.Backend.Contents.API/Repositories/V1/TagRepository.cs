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
        // Normalize and deduplicate the names
        names = names.Select(n => n.ToLower().Trim())
                    .Distinct()
                    .Where(n => !string.IsNullOrEmpty(n))
                    .ToList();

        if (!names.Any())
        {
            return new List<Tag>();
        }

        // Get all existing tags in one query with case-insensitive comparison
        var allExistingTags = await base.GetQueryable()
            .Where(t => names.Contains(t.TagName.ToLower().Trim()))
            .ToListAsync();

        // Handle potential duplicates in the database by taking the first tag for each name
        var existingTags = new List<Tag>();
        var processedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var tag in allExistingTags)
        {
            var normalizedName = tag.TagName.ToLower().Trim();
            if (!processedNames.Contains(normalizedName))
            {
                existingTags.Add(tag);
                processedNames.Add(normalizedName);
            }
        }

        // Create a dictionary of existing tags by normalized name for faster lookup
        var existingTagsByName = existingTags
            .ToDictionary(
                t => t.TagName.ToLower().Trim(),
                t => t,
                StringComparer.OrdinalIgnoreCase
            );

        // Find names that don't exist yet
        var tagsToCreate = names
            .Where(name => !existingTagsByName.ContainsKey(name))
            .ToList();

        // Create new tags if needed
        foreach (var name in tagsToCreate)
        {
            try
            {
                var tag = new Tag { TagName = name };
                var id = await CreateAsync(tag);
                tag.Id = id;
                existingTags.Add(tag);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear(); // Clear the context to avoid tracking conflicts

                // Try to find the tag again - it might have been created by another process
                // or there might be duplicates in the DB
                var existingTag = await base.GetQueryable()
                    .Where(t => t.TagName.ToLower().Trim() == name.ToLower().Trim())
                    .OrderBy(t => t.Id) // Take the first one if there are duplicates
                    .FirstOrDefaultAsync();

                if (existingTag != null && !existingTags.Any(t => t.Id == existingTag.Id))
                {
                    existingTags.Add(existingTag);
                }
            }
        }

        return existingTags;
    }
}
