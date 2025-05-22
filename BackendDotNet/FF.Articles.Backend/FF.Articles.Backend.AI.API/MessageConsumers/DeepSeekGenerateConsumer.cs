using RabbitMQ.Client;

namespace FF.Articles.Backend.AI.API.MessageConsumers;

public abstract class DeepSeekGenerateConsumer : BaseConsumer
{
    private static bool IsProduction => EnvUtil.IsProduction();
    private Timer _windowCheckTimer;

    public DeepSeekGenerateConsumer(
        IConnection connection,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<DeepSeekGenerateConsumer> logger,
        string queueName)
        : base(connection, serviceScopeFactory, logger, queueName, prefetchCount: 3)
    {
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        QueueDeclareHelper.DeclareQueues(_channel, _queue);

        SetupConsumers();

        StartConsumingDlq();

        if (!IsProduction)
        {
            StartConsuming();
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
            StartConsuming();
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
        await GenerateArticleContentAsync(messageJson);
    }

    protected override async Task HandleDeadLetterMessageAsync(string messageJson)
    {
        _logger.LogWarning("Processing dead letter message: {messageJson}", messageJson);
        await GenerateArticleContentAsync(messageJson, true);
    }

    protected abstract Task GenerateArticleContentAsync(string messageJson, bool isDeadLetter = false);

    public override void Dispose()
    {
        _windowCheckTimer?.Dispose();
        base.Dispose();
    }
}
