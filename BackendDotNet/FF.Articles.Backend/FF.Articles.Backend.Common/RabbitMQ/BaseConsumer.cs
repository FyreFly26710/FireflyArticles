using Microsoft.Extensions.Hosting;

namespace FF.Articles.Backend.Common.RabbitMQ;

public abstract class BaseConsumer : BackgroundService
{
    private readonly IConnection _connection;
    protected readonly IModel _channel;
    protected readonly IServiceScopeFactory _serviceScopeFactory;
    protected readonly string _queue;
    protected readonly ILogger<BaseConsumer> _logger;
    private EventingBasicConsumer _consumer;
    private EventingBasicConsumer _dlqConsumer;

    public BaseConsumer(
        IConnection connection,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<BaseConsumer> logger,
        string queue,
        int prefetchCount = 3)
    {
        _connection = connection;
        _channel = _connection.CreateModel();
        _serviceScopeFactory = serviceScopeFactory;
        _queue = queue;
        _logger = logger;
        // _prefetchCount = prefetchCount;

        // Set prefetch in constructor to ensure it's applied before any consumer setup
        _channel.BasicQos(prefetchSize: 0, prefetchCount: (ushort)prefetchCount, global: false);
        _logger.LogInformation("Configured prefetch count: {prefetchCount} for queue: {queue}", prefetchCount, _queue);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        QueueDeclareHelper.DeclareQueues(_channel, _queue);

        // Set up consumers
        SetupConsumers();

        StartConsuming();
        StartConsumingDlq();

        return Task.CompletedTask;
    }

    protected void SetupConsumers()
    {
        // Set up consumer for main queue
        _consumer = new EventingBasicConsumer(_channel);
        _consumer.Received += async (model, ea) =>
        {
            var messageJson = QueueDeclareHelper.GetJsonMessage(ea);
            var logMessage = $"{messageJson.Substring(0, Math.Min(messageJson.Length, 500))}...";
            _logger.LogInformation("Received message: {logMessage}", logMessage);

            try
            {
                await HandleMessageAsync(messageJson);

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                _logger.LogInformation("Message processed: {messageJson}", logMessage);
            }
            catch (Exception ex)
            {
                var deathCount = QueueDeclareHelper.GetDeathCount(ea);
                _logger.LogError(ex, "Error processing message: {messageJson}, Death count: {deathCount}", logMessage, deathCount);

                if (deathCount >= 3)
                {
                    // After 3 attempts, move to DLQ manually
                    _channel.BasicPublish(exchange: "", routingKey: $"{_queue}.dlq", basicProperties: null, body: ea.Body);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                else
                {
                    // Retry (send to retry queue automatically via nack)
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            }
        };

        // Set up DLQ consumer
        _dlqConsumer = new EventingBasicConsumer(_channel);
        _dlqConsumer.Received += async (model, ea) =>
        {
            var messageJson = QueueDeclareHelper.GetJsonMessage(ea);
            var logMessage = $"{messageJson.Substring(0, Math.Min(messageJson.Length, 500))}...";
            try
            {
                // Call the DLQ-specific handler method
                await HandleDeadLetterMessageAsync(messageJson);
                _logger.LogInformation("DLQ message processed: {logMessage}", logMessage);

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing DLQ message: {logMessage}", logMessage);
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        };
    }
    protected string StartConsuming() => _channel.BasicConsume(queue: _queue, autoAck: false, consumer: _consumer);
    protected string StartConsumingDlq() => _channel.BasicConsume(queue: $"{_queue}.dlq", autoAck: false, consumer: _dlqConsumer);

    protected abstract Task HandleMessageAsync(string messageJson);
    protected virtual Task HandleDeadLetterMessageAsync(string messageJson)
    {
        _logger.LogWarning("HandleDeadLetterMessageAsync not implemented for message: {messageJson}", messageJson);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        try
        {
            if (_channel?.IsOpen == true)
            {
                _channel.Close();
            }
            if (_connection?.IsOpen == true)
            {
                _connection.Close();
            }
        }
        catch (Exception ex)
        {
            // Log the error but don't throw it during disposal
            _logger.LogError(ex, "Error occurred while disposing RabbitMQ connection");
        }
        finally
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
