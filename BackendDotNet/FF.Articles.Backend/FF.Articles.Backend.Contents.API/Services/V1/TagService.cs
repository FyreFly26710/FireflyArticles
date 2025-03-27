using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V1;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Services.V1;
public class TagService : BaseService<Tag, ContentsDbContext>, ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly IContentsUnitOfWork _contentsUnitOfWork;
    private readonly IArticleTagRepository _articleTagRepository;
    public TagService(
        ITagRepository tagRepository,
        IArticleTagRepository articleTagRepository,
        IContentsUnitOfWork contentsUnitOfWork,
        ILogger<TagService> logger
    )
    : base(tagRepository, logger)
    {
        _contentsUnitOfWork = contentsUnitOfWork;
        _tagRepository = tagRepository;
        _articleTagRepository = articleTagRepository;
    }
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
