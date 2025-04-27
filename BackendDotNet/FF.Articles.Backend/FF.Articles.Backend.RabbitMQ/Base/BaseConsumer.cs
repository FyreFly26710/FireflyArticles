using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace FF.Articles.Backend.RabbitMQ;

public abstract class BaseConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    protected readonly IServiceScopeFactory _serviceScopeFactory;
    protected readonly string _queue;
    protected readonly ILogger<BaseConsumer> _logger;

    public BaseConsumer(
        IConnection connection,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<BaseConsumer> logger,
        string queue)
    {
        _connection = connection;
        _channel = _connection.CreateModel();
        _serviceScopeFactory = serviceScopeFactory;
        _queue = queue;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        QueueDeclareHelper.DeclareQueues(_channel, _queue);

        // Set up consumer for main queue
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var messageJson = QueueDeclareHelper.GetJsonMessage(ea);

            try
            {
                await HandleMessageAsync(messageJson);

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                _logger.LogInformation("Message processed: {messageJson}", messageJson);
            }
            catch (Exception ex)
            {
                retryMessage(ex, ea);
            }
        };
        _logger.LogInformation("Starting to listen on queue: {queue}", _queue);
        _channel.BasicConsume(queue: _queue, autoAck: false, consumer: consumer);

        var dlqConsumer = new EventingBasicConsumer(_channel);
        dlqConsumer.Received += async (model, ea) =>
        {
            try
            {
                var messageJson = QueueDeclareHelper.GetJsonMessage(ea);

                // Call the DLQ-specific handler method
                await HandleDeadLetterMessageAsync(messageJson);
                _logger.LogInformation("DLQ message processed: {messageJson}", messageJson);

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing DLQ message: {messageJson}", QueueDeclareHelper.GetJsonMessage(ea));
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        };

        string dlqName = $"{_queue}.dlq";
        _logger.LogInformation("Starting to listen on DLQ: {dlqName}", dlqName);
        _channel.BasicConsume(queue: dlqName, autoAck: false, consumer: dlqConsumer);

        return Task.CompletedTask;
    }

    protected abstract Task HandleMessageAsync(string messageJson);

    // New method to handle dead letter messages
    // Implement this in your derived class to handle DLQ messages
    protected virtual Task HandleDeadLetterMessageAsync(string messageJson)
    {
        _logger.LogWarning("HandleDeadLetterMessageAsync not implemented for message: {messageJson}", messageJson);
        return Task.CompletedTask;
    }

    private void retryMessage(Exception ex, BasicDeliverEventArgs ea)
    {

        var deathCount = QueueDeclareHelper.GetDeathCount(ea);
        _logger.LogError(ex, "Error processing message: {messageJson}, Death count: {deathCount}", QueueDeclareHelper.GetJsonMessage(ea), deathCount);

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

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
