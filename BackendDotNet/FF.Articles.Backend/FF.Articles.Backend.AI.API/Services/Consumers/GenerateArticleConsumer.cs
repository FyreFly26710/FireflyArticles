using System;
using System.Text;
using System.Text.Json;
using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;
using RabbitMQ.Client;
using FF.Articles.Backend.RabbitMQ;
using FF.Articles.Backend.AI.API.MapperExtensions;
using FF.Articles.Backend.Common.ApiDtos;

namespace FF.Articles.Backend.AI.API.Services.Consumers;

public class GenerateArticleConsumer(IConnection connection, IServiceScopeFactory serviceScopeFactory, ILogger<GenerateArticleConsumer> logger)
    : BaseConsumer(connection, serviceScopeFactory, logger, QueueList.GenerateArticleQueue)
{

    protected override async Task HandleMessageAsync(string messageJson)
    {
        await PublishArticleReadyMessage(messageJson);
    }
    protected override async Task HandleDeadLetterMessageAsync(string messageJson)
    {
        await PublishArticleReadyMessage(messageJson, true);
    }

    private async Task PublishArticleReadyMessage(string messageJson, bool isDeadLetter = false)
    {
        var generateRequest = JsonSerializer.Deserialize<ContentRequest>(messageJson);

        if (generateRequest is not null)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            string articleContent = "Error generating article";
            if (!isDeadLetter)
            {
                var articleGenerationService = scope.ServiceProvider.GetRequiredService<IArticleGenerationService>();
                articleContent = await articleGenerationService.GenerateArticleContentAsync(generateRequest);
            }
            var newRequest = generateRequest.ToArticleApiUpsertRequest(articleContent);

            var rabbitMqPublisher = scope.ServiceProvider.GetRequiredService<IRabbitMqPublisher>();
            await rabbitMqPublisher.PublishAsync(QueueList.ArticleReadyQueue, newRequest);
        }
    }
}
