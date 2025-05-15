using System.Text.Json;
using FF.Articles.Backend.Common.RabbitMQ;
using RabbitMQ.Client;

namespace FF.Articles.Backend.Contents.API.MessageConsumers;

public class UpdateArticleConsumer(IConnection connection, IServiceScopeFactory serviceScopeFactory, ILogger<UpdateArticleConsumer> logger)
    : BaseConsumer(connection, serviceScopeFactory, logger, QueueList.EditArticleQueue)
{

    protected override async Task HandleMessageAsync(string messageJson)
    {
        var message = JsonSerializer.Deserialize<ArticleEditRequest>(messageJson);
        if (message is null)
        {
            logger.LogError("Invalid message: {messageJson}", messageJson);
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var articleService = scope.ServiceProvider.GetRequiredService<IArticleService>();

        var articleId = message.ArticleId;
        var article = await articleService.GetByIdAsync(articleId);
        if (article is null)
        {
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, $"Article not found {articleId}, title: {message.Title}");
        }
        else
        {
            await articleService.EditArticleByRequest(message);
        }

    }
}
