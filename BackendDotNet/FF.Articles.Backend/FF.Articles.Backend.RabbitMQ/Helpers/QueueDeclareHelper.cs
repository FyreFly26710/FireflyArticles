using System;
using RabbitMQ.Client;

namespace FF.Articles.Backend.RabbitMQ;

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
        channel.QueueDeclare(queue: queueName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: mainQueueArgs);

        // Declare retry queue
        channel.QueueDeclare(queue: $"{queueName}.retry",
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: retryQueueArgs);

        // Declare DLQ for final failures
        channel.QueueDeclare(queue: $"{queueName}.dlq",
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
        return channel;
    }
}
