namespace FF.Articles.Backend.Contents.API.Repositories;
public class TagRepository(ContentsDbContext _context)
    : BaseRepository<Tag, ContentsDbContext>(_context), ITagRepository
{
    public async Task<List<Tag>> GetByNamesAsync(List<string> names)
    {
        names = names.Select(n => n.Trim().ToLower()).Distinct().ToList();
        names = names.Where(n => !string.IsNullOrEmpty(n)).ToList();
        return await base.GetQueryable().Where(t => names.Contains(t.TagName.Normalize())).ToListAsync();
    }

    public async Task<List<Tag>> GetOrCreateByNamesAsync(List<string> names)
    {
        // Normalize and deduplicate the names
        names = names.Select(n => n.Trim().ToLower())
                    .Distinct()
                    .Where(n => !string.IsNullOrEmpty(n))
                    .ToList();

        if (names.Count == 0) return [];


        // Get all existing tags in one query with case-insensitive comparison
        // Db may have duplicates, so we need to handle them by taking the first tag for each name
        var allExistingTags = await base.GetQueryable()
            .Where(t => names.Contains(t.TagName.Trim().ToLower()))
            .ToListAsync();

        // Handle potential duplicates in the database by taking the first tag for each name
        var existingTags = new List<Tag>();
        var processedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tag in allExistingTags)
        {
            var normalizedName = tag.TagName.Trim().ToLower();
            if (!processedNames.Contains(normalizedName))
            {
                existingTags.Add(tag);
                processedNames.Add(normalizedName);
            }
        }

        var tagsToCreate = names
            .Where(name => !existingTags.Any(t => t.TagName.Trim().ToLower() == name.Trim().ToLower()))
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
