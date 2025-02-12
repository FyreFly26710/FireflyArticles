using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Services.Interfaces;

namespace FF.Articles.Backend.Contents.API.Services;
public class TopicService(ContentsDbContext _context, ILogger<TopicService> _logger, IMapper _mapper)
: CacheService<Topic, ContentsDbContext>(_context, _logger), ITopicService
{
}

