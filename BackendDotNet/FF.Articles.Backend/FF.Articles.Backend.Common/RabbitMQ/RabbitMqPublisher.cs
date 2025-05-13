using System.Text;

namespace FF.Articles.Backend.Common.RabbitMQ;

public interface IRabbitMqPublisher
{
    void Publish<T>(string queueName, T message);
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

    public void Publish<T>(string queueName, T message)
    {
        QueueDeclareHelper.DeclareQueues(_channel, queueName);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
    }
}
