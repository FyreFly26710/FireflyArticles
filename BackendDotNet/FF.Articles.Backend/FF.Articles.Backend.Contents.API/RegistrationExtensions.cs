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
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Controller stays the same, V1 and V2 services share the same interface
            RegisteredServices<IArticleService, Services.V1.ArticleService, Services.V2.ArticleService>(services);
            RegisteredServices<ITagService, Services.V1.TagService, Services.V2.TagService>(services);
            RegisteredServices<ITopicService, Services.V1.TopicService, Services.V2.TopicService>(services);
            services.AddScoped<IIdentityRemoteService, IdentityRemoteService>();

            // V1 repositories
            services.AddScoped<IContentsUnitOfWork, ContentsUnitOfWork>();

            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ITopicRepository, TopicRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IArticleTagRepository, ArticleTagRepository>();

            // V2 repositories
            services.AddScoped<IArticleRedisRepository, ArticleRedisRepository>();
            services.AddScoped<ITopicRedisRepository, TopicRedisRepository>();
            services.AddScoped<ITagRedisRepository, TagRedisRepository>();
            services.AddScoped<IArticleTagRedisRepository, ArticleTagRedisRepository>();


            return services;
        }
        private static void RegisteredServices<TInterface, Tv1, Tv2>(IServiceCollection services)
        where TInterface : class
        where Tv1 : class, TInterface
        where Tv2 : class, TInterface
        {
            services.AddScoped<Tv1>();
            services.AddScoped<Tv2>();
            services.AddScoped<Func<string, TInterface>>(serviceProvider => key =>
               {
                   return key switch
                   {
                       "v1" => serviceProvider.GetRequiredService<Tv1>(),
                       "v2" => serviceProvider.GetRequiredService<Tv2>(),
                       _ => throw new ArgumentException("Invalid version", nameof(key))
                   };
               });
        }
    }
}
