using System.Text.Json;
using FF.Articles.Backend.Common.RabbitMQ;
using RabbitMQ.Client;

namespace FF.Articles.Backend.Contents.API.MqConsumers;

public class UpdateArticleConsumer(IConnection connection, IServiceScopeFactory serviceScopeFactory, ILogger<UpdateArticleConsumer> logger)
    : BaseConsumer(connection, serviceScopeFactory, logger, QueueList.EditArticleQueue)
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

        var articleId = generateRequest.Id ?? 0L;
        var article = await articleService.GetByIdAsync(articleId);
        if (article is null)
        {
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, $"Article not found {articleId}, title: {generateRequest.Title}");
        }
        else
        {
            var editRequest = MapToEditRequest(generateRequest, true);
            await articleService.EditArticleByRequest(editRequest);
        }

    }
    private ArticleEditRequest MapToEditRequest(ArticleApiUpsertRequest generateRequest, bool partialUpdate)
    {
        var editRequest = new ArticleEditRequest
        {
            ArticleId = (long)generateRequest.Id!,
            Content = generateRequest.Content,
        };

        if (!partialUpdate)
        {
            editRequest.Title = generateRequest.Title;
            editRequest.Abstract = generateRequest.Abstract;
            editRequest.ArticleType = generateRequest.ArticleType;
            editRequest.ParentArticleId = generateRequest.ParentArticleId;
            editRequest.TopicId = generateRequest.TopicId;
            editRequest.Tags = generateRequest.Tags;
            editRequest.SortNumber = generateRequest.SortNumber;
        }

        return editRequest;
    }
}
