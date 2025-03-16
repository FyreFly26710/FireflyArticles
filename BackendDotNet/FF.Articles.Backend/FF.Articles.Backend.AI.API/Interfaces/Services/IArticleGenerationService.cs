using System;
using FF.Articles.Backend.AI.API.Models.AiDtos;

namespace FF.Articles.Backend.AI.API.Interfaces;

public interface IArticleGenerationService
{
    Task<ArticlesAIResponse> GenerateArticleListsAsync(string topic, HttpRequest request, int articleCount = 8, CancellationToken cancellationToken = default);
    Task<string> GenerateArticleContentAsync(int articleId, HttpRequest request, CancellationToken cancellationToken = default);
    Task BatchGenerateArticleContentAsync(List<int> articleIds, HttpRequest httpRequest, CancellationToken cancellationToken = default);
    
}
