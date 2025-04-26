using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace FF.Articles.Backend.RabbitMQ;

public static class RegistrationExtensions
{
    /// <summary>
    /// Get a RabbitMQ connection from the configuration (ConnectionStrings__rabbitmq)
    /// Register a singleton IRabbitMqPublisher
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddRabbitMq(this WebApplicationBuilder builder)
    {
        builder.AddRabbitMQClient(connectionName: "rabbitmq");
        builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
        return builder;
    }
}
