using Microsoft.Extensions.DependencyInjection;

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
