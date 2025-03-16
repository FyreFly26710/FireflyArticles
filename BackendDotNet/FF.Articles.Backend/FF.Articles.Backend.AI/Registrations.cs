using System;
using FF.Articles.Backend.AI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using FF.Articles.Backend.AI.Constants;

namespace FF.Articles.Backend.AI;

public static class Registrations
{
    public static IServiceCollection AddDeepSeek(this IServiceCollection services, IConfiguration configuration)
    {
        var apiKey = configuration["DeepSeek:ApiKey"];
        var baseUrl = new Uri(ApiEndpoints.DeepSeekBaseAddress);
        services.AddHttpClient("DeepSeekClient", client => 
        {
            client.BaseAddress = baseUrl;
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", apiKey);
            client.Timeout = TimeSpan.FromSeconds(120);
        });

        services.AddScoped<IDeepSeekClient, DeepSeekClient>(sp => 
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("DeepSeekClient");
            return new DeepSeekClient(httpClient);
        });
        return services;
    }
}
