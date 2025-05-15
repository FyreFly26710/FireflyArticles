using RabbitMQ.Client;

namespace FF.Articles.Backend.AI.API.Services.Consumers;

public class GenerateArticleConsumer : BaseConsumer
{
    private static bool IsProduction => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
    private Timer _windowCheckTimer;

    public GenerateArticleConsumer(
        IConnection connection,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<GenerateArticleConsumer> logger)
        : base(connection, serviceScopeFactory, logger, QueueList.GenerateArticleQueue, prefetchCount: 3)
    {
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        QueueDeclareHelper.DeclareQueues(_channel, _queue);

        base.SetupConsumers();

        base.StartConsumingDlq();

        if (!IsProduction)
        {
            base.StartConsuming();
            _logger.LogInformation("Started consuming main queue (non-production environment)");
        }

        if (IsProduction)
        {
            _windowCheckTimer = new Timer(CheckProcessingWindow, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        return Task.CompletedTask;
    }

    private void CheckProcessingWindow(object state)
    {
        var isWithinWindow = IsWithinProcessingWindow();
        _logger.LogInformation("Checking processing window.");

        if (isWithinWindow)
        {
            base.StartConsuming();
            _logger.LogInformation("within processing window");
        }
        else
        {
            _logger.LogInformation("outside processing window");
        }
    }

    /// <summary>
    /// DeepSeek API Discounts are available between 16:30 and 00:30 UTC. <br/>
    /// </summary>
    private bool IsWithinProcessingWindow()
    {
        if (!IsProduction)
            return true; // Always process in non-production environments

        var utcNow = DateTime.UtcNow;
        var startTime = new TimeSpan(16, 30, 0); // 16:30 UTC
        var endTime = new TimeSpan(0, 30, 0);    // 00:30 UTC

        var currentTime = utcNow.TimeOfDay;

        // Check if current time is between start and midnight
        if (startTime <= currentTime && currentTime <= TimeSpan.FromHours(24))
            return true;

        // Check if current time is between midnight and end
        if (TimeSpan.Zero <= currentTime && currentTime <= endTime)
            return true;

        return false;
    }

    protected override async Task HandleMessageAsync(string messageJson)
    {
        await PublishArticleReadyMessage(messageJson);
    }

    protected override async Task HandleDeadLetterMessageAsync(string messageJson)
    {
        _logger.LogWarning("Processing dead letter message: {messageJson}", messageJson);
        await PublishArticleReadyMessage(messageJson, true);
    }

    private async Task PublishArticleReadyMessage(string messageJson, bool isDeadLetter = false)
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

    public override void Dispose()
    {
        _windowCheckTimer?.Dispose();
        base.Dispose();
    }
}
