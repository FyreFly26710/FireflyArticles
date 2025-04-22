using System;
using FF.Articles.Backend.AI.API.Models.Dtos;
using FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;
using FF.Articles.Backend.Common.ApiDtos;

namespace FF.Articles.Backend.AI.API.Interfaces.Services;

public interface IArticleGenerationService
{
    Task<ArticlesAIResponseDto> GenerateArticleListsAsync(ArticleListRequest request, CancellationToken cancellationToken = default);
    Task<long> GenerateArticleContentAsync(ContentRequest request);
}
