using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V1;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.Contents.API.Repositories.V1;
using FF.Articles.Backend.Contents.API.Repositories.V2;
using FF.Articles.Backend.Contents.API.Services.RemoteServices;
using FF.Articles.Backend.Contents.API.UnitOfWork;
using Microsoft.EntityFrameworkCore.Internal;

namespace FF.Articles.Backend.Contents.API
{
    public static class RegistrationExtensions
    {
        public static IServiceCollection AddContentsServices(this IServiceCollection services)
        {
            // Controller stays the same, V1 and V2 services share the same interface
            RegisteredServices<IArticleService, Services.V1.ArticleService, Services.V2.ArticleService>(services);
            RegisteredServices<ITagService, Services.V1.TagService, Services.V2.TagService>(services);
            RegisteredServices<ITopicService, Services.V1.TopicService, Services.V2.TopicService>(services);

            // Add HttpClient for IdentityRemoteService
            services.AddHttpClient<IIdentityRemoteService, IdentityRemoteService>();

            // V1 repositories
            services.AddScoped<IContentsUnitOfWork, ContentsUnitOfWork>();

            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ITopicRepository, TopicRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IArticleTagRepository, ArticleTagRepository>();

            // V2 repositories (Redis)
            services.AddScoped<ITopicRedisRepository, TopicRedisRepository>();
            services.AddScoped<IArticleRedisRepository, ArticleRedisRepository>();
            services.AddScoped<ITagRedisRepository, TagRedisRepository>();
            services.AddScoped<IArticleTagRedisRepository, ArticleTagRedisRepository>();

            return services;
        }

        /// <summary>
        /// Used to register V1(EF with sql) and V2(Redis) services with same interface
        /// </summary>
        /// <typeparam name="I">Interface</typeparam>
        /// <typeparam name="S1">V1 Service</typeparam>
        /// <typeparam name="S2">V2 Service</typeparam>
        /// <param name="services"></param>
        public static void RegisteredServices<I, S1, S2>(IServiceCollection services)
            where I : class
            where S1 : class, I
            where S2 : class, I
        {
            services.AddScoped<S1>();
            services.AddScoped<S2>();

            // When injected without functional parameter, use V1 by default
            services.AddScoped<I>(provider => provider.GetRequiredService<S1>());

            // When using function parameter, use service passed in
            services.AddScoped<Func<string, I>>(serviceProvider => key =>
            {
                return key switch
                {
                    "v1" => serviceProvider.GetService<S1>()!,
                    "v2" => serviceProvider.GetService<S2>()!,
                    _ => serviceProvider.GetService<S1>()!,
                };
            });
        }
    }
}
