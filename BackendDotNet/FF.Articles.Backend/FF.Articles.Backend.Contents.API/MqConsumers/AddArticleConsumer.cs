using System.Text.Json;
using FF.Articles.Backend.Common.RabbitMQ;
using RabbitMQ.Client;

namespace FF.Articles.Backend.Contents.API.MqConsumers;

/// <summary>
/// Set prefetch count to 1 to ensure that the message is processed in order
/// </summary>
public class AddArticleConsumer(IConnection connection, IServiceScopeFactory serviceScopeFactory, ILogger<AddArticleConsumer> logger)
    : BaseConsumer(connection, serviceScopeFactory, logger, QueueList.AddArticleQueue, prefetchCount: 1)
{

    protected override async Task HandleMessageAsync(string messageJson)
    {
        var generateRequest = JsonSerializer.Deserialize<ArticleApiUpsertRequest>(messageJson);
        if (generateRequest is null)
        {
            logger.LogError("Invalid message: {messageJson}", messageJson);
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var articleService = scope.ServiceProvider.GetRequiredService<IArticleService>();

        var articleId = generateRequest.ArticleId ?? 0L;
        var article = await articleService.GetByIdAsync(articleId);
        if (article is null)
        {
            var createRequest = MapToCreateRequest(generateRequest);
            await articleService.CreateByRequest(createRequest, generateRequest.UserId ?? AdminUsers.SYSTEM_ADMIN_FIREFLY.UserId);
        }

    }
    private ArticleAddRequest MapToCreateRequest(ArticleApiUpsertRequest generateRequest)
    {
        var createRequest = new ArticleAddRequest
        {
            ArticleId = generateRequest.ArticleId,
            Title = generateRequest.Title,
            Abstract = generateRequest.Abstract,
            ArticleType = generateRequest.ArticleType,
            ParentArticleId = generateRequest.ParentArticleId,
            TopicId = generateRequest.TopicId,
            Tags = generateRequest.Tags ?? new List<string>(),
            SortNumber = generateRequest.SortNumber,
            Content = generateRequest.Content,
        };

        return createRequest;
    }
}
