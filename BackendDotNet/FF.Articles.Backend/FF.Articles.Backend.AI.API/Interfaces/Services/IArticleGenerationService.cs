using System;
using FF.Articles.Backend.AI.API.Models.AiDtos;
using FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;
using FF.Articles.Backend.Common.ApiDtos;

namespace FF.Articles.Backend.AI.API.Interfaces;

public interface IArticleGenerationService
{
    Task<ArticlesAIResponseDto> GenerateArticleListsAsync(ArticleListRequest request, CancellationToken cancellationToken = default);
    Task<long> GenerateArticleContentAsync(ContentRequest request);
}
