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

    // public async Task<List<Tag>> GetOrCreateByNamesAsync(List<string> names)
    // {
    //     // Normalize and deduplicate the names
    //     names = names.Select(n => n.Trim().ToLower())
    //                 .Distinct()
    //                 .Where(n => !string.IsNullOrEmpty(n))
    //                 .ToList();

    //     if (names.Count == 0) return [];

    //     // Get all existing tags in one query with case-insensitive comparison
    //     // Db may have duplicates, so we need to handle them by taking the first tag for each name
    //     var allExistingTags = await base.GetQueryable()
    //         .Where(t => names.Contains(t.TagName.Trim().ToLower()))
    //         .ToListAsync();

    //     // Handle potential duplicates in the database by taking the first tag for each name
    //     var existingTags = new List<Tag>();
    //     var processedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    //     foreach (var tag in allExistingTags)
    //     {
    //         var normalizedName = tag.TagName.Trim().ToLower();
    //         if (!processedNames.Contains(normalizedName))
    //         {
    //             existingTags.Add(tag);
    //             processedNames.Add(normalizedName);
    //         }
    //     }

    //     var tagsToCreate = names
    //         .Where(n => !existingTags.Any(t => t.TagName.Normalize() == n.Normalize()))
    //         .ToList();

    //     // Create new tags if needed
    //     foreach (var tagName in tagsToCreate)
    //     {
    //         var tag = new Tag { TagName = tagName };
    //         var id = await CreateAsync(tag);
    //         tag.Id = id;
    //         existingTags.Add(tag);
    //     }

    //     return existingTags;
    // }


    public async Task<List<Tag>> GetOrCreateByNamesAsync(List<string> names)
    {
        // Normalize the names
        names = names.Select(n => n.Trim().ToLower()).Distinct().ToList();

        // Tags should be created by AI following exact order: Skill Level, Focus Area, Tech Stack/Language, Article Style, Tone
        // See FF.Articles.Backend.AI.API.Services.Prompts.TagRules
        List<string> preDefinedGroups = ["Skill Level", "Article Style", "Tone"];
        List<string> aiGroups = ["Focus Area", "Tech Stack/Language"];

        var preDefinedTags = new List<Tag>();
        var aiTags = new List<Tag>();
        if (names.Count == 5)
        {
            preDefinedTags.Add(new Tag { TagName = names[0], TagGroup = preDefinedGroups[0] });
            aiTags.Add(new Tag { TagName = names[1], TagGroup = aiGroups[0] });
            aiTags.Add(new Tag { TagName = names[2], TagGroup = aiGroups[1] });
            preDefinedTags.Add(new Tag { TagName = names[3], TagGroup = preDefinedGroups[1] });
            preDefinedTags.Add(new Tag { TagName = names[4], TagGroup = preDefinedGroups[2] });
        }
        else if (names.Count == 4)
        {
            preDefinedTags.Add(new Tag { TagName = names[0], TagGroup = preDefinedGroups[0] });
            aiTags.Add(new Tag { TagName = names[1], TagGroup = aiGroups[0] });
            preDefinedTags.Add(new Tag { TagName = names[2], TagGroup = preDefinedGroups[1] });
            preDefinedTags.Add(new Tag { TagName = names[3], TagGroup = preDefinedGroups[2] });
        }

        // Remove spaces from tag names
        preDefinedTags = preDefinedTags.Select(t => new Tag
        {
            TagName = t.TagName.Replace(" ", ""),
            TagGroup = t.TagGroup
        }).ToList();

        var existingTags = await base.GetQueryable()
            .Where(t => preDefinedTags.Any(p => p.TagName.ToLower() == t.TagName.ToLower() && p.TagGroup == t.TagGroup))
            .ToListAsync();
        var result = new List<Tag>();
        if (existingTags.Count != 3)
        {
            // AI does not follow the rules, so we need to use the regular approach
            foreach (var tagName in names)
            {
                var newTag = await GetOrCreateByNameAsync(tagName);
                result.Add(newTag);
            }
            return result;
        }
        else
        {
            result = [.. existingTags];
            foreach (var tag in aiTags)
            {
                var newTag = await GetOrCreateByNameAndGroupAsync(tag.TagName, tag.TagGroup!);
                result.Add(newTag);
            }
            return result;
        }
    }
    private async Task<Tag> GetOrCreateByNameAndGroupAsync(string name, string group)
    {
        var tag = await base.GetQueryable()
            .FirstOrDefaultAsync(t => t.TagName.ToLower() == name.ToLower() && t.TagGroup == group);

        if (tag == null)
        {
            tag = new Tag { TagName = name, TagGroup = group };
            var id = await CreateAsync(tag);
            tag.Id = id;
        }

        return tag;
    }
    private async Task<Tag> GetOrCreateByNameAsync(string name)
    {
        var tag = await base.GetQueryable()
            .FirstOrDefaultAsync(t => t.TagName.ToLower() == name.ToLower());

        if (tag == null)
        {
            tag = new Tag { TagName = name };
            var id = await CreateAsync(tag);
            tag.Id = id;
        }
        return tag;
    }

}
