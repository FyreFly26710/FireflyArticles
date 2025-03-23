﻿using FF.Articles.Backend.Common.Bases.Interfaces;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2
{
    public interface IArticleRedisRepository : IRedisRepository<Article>
    {
    }
}
