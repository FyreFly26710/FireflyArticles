using RabbitMQ.Client;

namespace FF.Articles.Backend.AI.API.MessageConsumers;

public class GenerateArticleListConsumer : DeepSeekGenerateConsumer
{

    public GenerateArticleListConsumer(
        IConnection connection,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<GenerateArticleListConsumer> logger)
        : base(connection, serviceScopeFactory, logger, QueueList.GenerateArticleListQueue)
    {
    }
    /// <summary>
    /// Todo: Add a queue to generate article list and distribute the articles to add article queue
    /// </summary>
    protected override async Task GenerateArticleContentAsync(string messageJson, bool isDeadLetter = false)
    {
        var generateRequest = JsonSerializer.Deserialize<ArticleListRequest>(messageJson);

        if (generateRequest is not null)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var contentsApiRemoteService = scope.ServiceProvider.GetRequiredService<IContentsApiRemoteService>();
            var articleGenerationService = scope.ServiceProvider.GetRequiredService<IArticleGenerationService>();

            var topic = await contentsApiRemoteService.GetTopicByTitleCategory(generateRequest.Topic, generateRequest.Category);
            var articleListJson = string.Empty;
            if (topic is not null)
            {
                articleListJson = await articleGenerationService.RegenerateArticleListAsync(generateRequest, topic);
            }
            else
            {
                articleListJson = await articleGenerationService.GenerateArticleListsAsync(generateRequest);
            }

            var articleList = JsonSerializer.Deserialize<List<ArticlesAIResponseDto>>(articleListJson);

            var rabbitMqPublisher = scope.ServiceProvider.GetRequiredService<IRabbitMqPublisher>();
            rabbitMqPublisher.Publish(QueueList.GenerateArticleListQueue, generateRequest);

        }
    }

}
