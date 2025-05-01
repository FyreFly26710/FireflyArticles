using System;
using System.Text;
using System.Text.Json;
using FF.Articles.Backend.RabbitMQ.Helpers;
using RabbitMQ.Client;

namespace FF.Articles.Backend.RabbitMQ.Base;

public interface IRabbitMqPublisher
{
    Task PublishAsync<T>(string queueName, T message);
}
public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqPublisher(IConnection connection)
    {
        _connection = connection;
        _channel = _connection.CreateModel();
    }

    public async Task PublishAsync<T>(string queueName, T message)
    {
        QueueDeclareHelper.DeclareQueues(_channel, queueName);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
    }
}
