﻿using FF.Articles.Backend.Common.Bases.Interfaces;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.Interfaces.Services;
public interface ITagService : IBaseService<Tag>
{
    Task<Tag?> GetTagByNameAsync(string tagName);
}

