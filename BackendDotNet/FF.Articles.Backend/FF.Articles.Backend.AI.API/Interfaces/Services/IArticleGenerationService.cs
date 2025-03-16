using System;
using FF.Articles.Backend.AI.API.Models.AiDtos;

namespace FF.Articles.Backend.AI.API.Interfaces;

public interface IArticleGenerationService
{
    Task<ArticlesAIResponse> GenerateArticleListsAsync(string topic, int articleCount = 8, CancellationToken cancellationToken = default);
    Task<string> GenerateArticleContentAsync(int articleId, CancellationToken cancellationToken = default);
}
