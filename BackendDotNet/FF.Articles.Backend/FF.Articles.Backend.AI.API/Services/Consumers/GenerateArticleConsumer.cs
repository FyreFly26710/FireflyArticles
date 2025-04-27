using System;
using System.Text;
using System.Text.Json;
using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;
using RabbitMQ.Client;
using FF.Articles.Backend.RabbitMQ;
using FF.Articles.Backend.AI.API.MapperExtensions;
using FF.Articles.Backend.Common.ApiDtos;
using System.Threading;
using System.Threading.Tasks;

namespace FF.Articles.Backend.AI.API.Services.Consumers;

public class GenerateArticleConsumer : BaseConsumer
{
    private static bool IsProduction => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
    private Timer _windowCheckTimer;
    // This semaphore ensures only one message is processed at a time, regardless of prefetch setting
    private static readonly SemaphoreSlim _processingLock = new SemaphoreSlim(1, 1);

    public GenerateArticleConsumer(
        IConnection connection,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<GenerateArticleConsumer> logger)
        : base(connection, serviceScopeFactory, logger, QueueList.GenerateArticleQueue, prefetchCount: 1)
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
        }

        if (IsProduction)
        {
            if (IsWithinProcessingWindow())
                StartConsuming();

            _windowCheckTimer = new Timer(CheckProcessingWindow, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        return Task.CompletedTask;
    }

    private void CheckProcessingWindow(object state)
    {
        if (IsWithinProcessingWindow())
            StartConsuming();
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
        // Use semaphore to ensure only one message is processed at a time
        await _processingLock.WaitAsync();
        try
        {
            _logger.LogInformation("Starting to process message (sequential processing enforced)");
            await PublishArticleReadyMessage(messageJson);
            _logger.LogInformation("Finished processing message");
        }
        finally
        {
            _processingLock.Release();
        }
    }

    protected override async Task HandleDeadLetterMessageAsync(string messageJson)
    {
        // DLQ messages can be processed concurrently since they're just recording errors
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

    public override void Dispose()
    {
        _windowCheckTimer?.Dispose();
        base.Dispose();
    }
}
