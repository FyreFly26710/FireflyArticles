﻿using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.Services.Interfaces;
public interface IArticleTagService : IBaseService<ArticleTag, ContentsDbContext>
{
    Task<bool> EditArticleTags(int articleId, List<int> tagIds);
    Task<bool> RemoveArticleTags(int articleId);
    Task<List<String>> GetArticleTags(int articleId);
    Task<Dictionary<int, List<String>>> GetArticleTags(List<int> articleIds);
}
