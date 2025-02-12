using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Services.Interfaces;

namespace FF.Articles.Backend.Contents.API.Services;
public class TagService(ContentsDbContext _context, ILogger<TagService> _logger, IMapper _mapper)
: CacheService<Tag, ContentsDbContext>(_context, _logger), ITagService
{
}
