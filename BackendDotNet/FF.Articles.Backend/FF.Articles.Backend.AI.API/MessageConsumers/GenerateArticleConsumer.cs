using RabbitMQ.Client;

namespace FF.Articles.Backend.AI.API.MessageConsumers;

public class GenerateArticleConsumer : DeepSeekGenerateConsumer
{

    public GenerateArticleConsumer(
        IConnection connection,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<GenerateArticleConsumer> logger)
        : base(connection, serviceScopeFactory, logger, QueueList.GenerateArticleQueue)
    {
    }

    protected override async Task GenerateArticleContentAsync(string messageJson, bool isDeadLetter = false)
    {
        var generateRequest = JsonSerializer.Deserialize<ContentRequest>(messageJson);

        if (generateRequest is not null)
        {
            if (generateRequest.Id == null)
            {
                throw new ApiException(ErrorCode.SYSTEM_ERROR, "GenerateRequest.Id is null");
            }
            using var scope = _serviceScopeFactory.CreateScope();

            // If dead letter, we don't need to generate article content
            string articleContent = "Error generating article";
            if (!isDeadLetter)
            {
                var articleGenerationService = scope.ServiceProvider.GetRequiredService<IArticleGenerationService>();
                articleContent = await articleGenerationService.GenerateArticleContentAsync(generateRequest);
            }
            var newRequest = generateRequest.ToArticleApiUpsertRequest(articleContent);

            var rabbitMqPublisher = scope.ServiceProvider.GetRequiredService<IRabbitMqPublisher>();
            rabbitMqPublisher.Publish(QueueList.EditArticleQueue, newRequest);
        }
    }

}
