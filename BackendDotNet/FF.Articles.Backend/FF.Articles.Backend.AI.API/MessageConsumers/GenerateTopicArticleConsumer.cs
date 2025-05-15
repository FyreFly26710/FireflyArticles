using RabbitMQ.Client;

namespace FF.Articles.Backend.AI.API.MessageConsumers;

public class GenerateTopicArticleConsumer : DeepSeekGenerateConsumer
{

    public GenerateTopicArticleConsumer(
        IConnection connection,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<GenerateTopicArticleConsumer> logger)
        : base(connection, serviceScopeFactory, logger, QueueList.GenerateTopicArticleQueue)
    {
    }

    protected override async Task GenerateArticleContentAsync(string messageJson, bool isDeadLetter = false)
    {
        var generateRequest = JsonSerializer.Deserialize<TopicApiDto>(messageJson);

        if (generateRequest is not null)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            // If dead letter, we don't need to generate article content
            string articleContent = "Error generating topic article";
            if (!isDeadLetter)
            {
                var articleGenerationService = scope.ServiceProvider.GetRequiredService<IArticleGenerationService>();
                articleContent = await articleGenerationService.GenerateTopicContentAsync(generateRequest);
            }
            var newRequest = generateRequest.ToArticleApiUpsertRequest(articleContent);

            var rabbitMqPublisher = scope.ServiceProvider.GetRequiredService<IRabbitMqPublisher>();
            rabbitMqPublisher.Publish(QueueList.EditArticleQueue, newRequest);
        }
    }

}
