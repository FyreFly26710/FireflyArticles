using System;
using FF.AI.Common.Interfaces;
using FF.AI.Common.Providers;
using FF.AI.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using FF.AI.Common.Services.ChatAssistants;
namespace FF.AI.Common;

public static class Registrations
{
    public static IServiceCollection AddAI(this IServiceCollection services)
    {
        services.AddHttpClient("ai-client", client =>
        {
            client.Timeout = TimeSpan.FromMinutes(10);
        });

        services.AddSingleton<DeepSeekProvider>();
        services.AddSingleton<OllamaProvider>();
        services.AddSingleton<GeminiProvider>();

        services.AddScoped<IAssistant<DeepSeekProvider>, DeepSeekAssistant>();
        services.AddScoped<IAssistant<OllamaProvider>, OllamaAssistant>();
        services.AddScoped<IAssistant<GeminiProvider>, GeminiAssistant>();

        services.AddScoped<IAssistant, AiChatAssistant>();

        return services;
    }

}
