using System;
using FF.AI.Common.Interfaces;
using FF.AI.Common.Providers;
using FF.AI.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
namespace FF.AI.Common;

public static class Registrations
{
    public static IServiceCollection AddAI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<DeepSeekProvider>(sp => new DeepSeekProvider(configuration));
        services.AddSingleton<OllamaProvider>(sp => new OllamaProvider(configuration));

        services.AddSingleton<IAssistant<DeepSeekProvider>, DeepSeekAssistant>();
        services.AddSingleton<IAssistant<OllamaProvider>, OllamaAssistant>();
        return services;
    }
}
