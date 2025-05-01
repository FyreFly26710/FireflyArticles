using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FF.Articles.Backend.RabbitMQ.Helpers;

public class QueueDeclareHelper
{
    public static IModel DeclareQueues(IModel channel, string queueName)
    {
        var mainQueueArgs = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", $"{queueName}.retry" }
        };

        var retryQueueArgs = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", queueName }, // resend to the main queue
            { "x-message-ttl", 10000 } // 10 seconds delay
        };

        // Declare main queue
        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: mainQueueArgs);

        // Declare retry queue
        channel.QueueDeclare(queue: $"{queueName}.retry", durable: true, exclusive: false, autoDelete: false, arguments: retryQueueArgs);

        // Declare DLQ for final failures
        channel.QueueDeclare(queue: $"{queueName}.dlq", durable: true, exclusive: false, autoDelete: false, arguments: null);
        return channel;
    }

    public static int GetDeathCount(BasicDeliverEventArgs ea)
    {
        var headers = ea.BasicProperties.Headers;
        int deathCount = 0;
        if (headers != null && headers.TryGetValue("x-death", out var deathHeaderObj))
        {
            var deathHeader = (List<object>)deathHeaderObj;
            if (deathHeader.Count > 0)
            {
                var death = (Dictionary<string, object>)deathHeader[0];
                deathCount = (int)(long)death["count"];
            }
        }
        return deathCount;
    }
    public static string GetJsonMessage(BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var messageJson = Encoding.UTF8.GetString(body);
        return messageJson;
    }
}
