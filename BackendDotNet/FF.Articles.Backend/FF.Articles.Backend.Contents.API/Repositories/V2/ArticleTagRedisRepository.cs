using System;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Models.Entities;
using StackExchange.Redis;

namespace FF.Articles.Backend.Contents.API.Repositories.V2;

public class ArticleTagRedisRepository : RedisRepository<ArticleTag>, IArticleTagRedisRepository
{
    public ArticleTagRedisRepository(IConnectionMultiplexer redis, ILogger<ArticleTagRedisRepository> logger)
     : base(redis, logger, hasTime: false)
    {
    }
}
