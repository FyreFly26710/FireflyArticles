using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.UnitOfWork;

namespace FF.Articles.Backend.Contents.API.Services.V2;
public class TagService : BaseService<Tag, ContentsDbContext>, ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly IContentsUnitOfWork _contentsUnitOfWork;
    private readonly IArticleTagRepository _articleTagRepository;
    public TagService(
        Func<string, ITagRepository> tagRepository,
        Func<string, IArticleTagRepository> articleTagRepository,
        IContentsUnitOfWork contentsUnitOfWork,
        ILogger<TagService> logger
    )
    : base(tagRepository("v1"), logger)
    {
        _contentsUnitOfWork = contentsUnitOfWork;
        _tagRepository = tagRepository("v1");
        _articleTagRepository = articleTagRepository("v1");
    }
    public override async Task<bool> DeleteAsync(int id)
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
}
