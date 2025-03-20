using FF.Articles.Backend.Contents.API.Interfaces.Repositories;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.Contents.API.UnitOfWork;
using Microsoft.EntityFrameworkCore.Internal;

namespace FF.Articles.Backend.Contents.API
{
    public static class RegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            RegisteredServices<IArticleService, Services.V1.ArticleService, Services.V2.ArticleService>(services);
            RegisteredServices<ITagService, Services.V1.TagService, Services.V2.TagService>(services);
            RegisteredServices<IArticleTagService, Services.V1.ArticleTagService, Services.V2.ArticleTagService>(services);
            RegisteredServices<ITopicService, Services.V1.TopicService, Services.V2.TopicService>(services);

            RegisteredServices<IIdentityRemoteService, Services.V1.RemoteServices.IdentityRemoteService, Services.V2.RemoteServices.IdentityRemoteService>(services);

            RegisteredServices<IArticleRepository, Repositories.V1.ArticleRepository, Repositories.V2.ArticleRepository>(services);
            RegisteredServices<ITopicRepository, Repositories.V1.TopicRepository, Repositories.V2.TopicRepository>(services);
            RegisteredServices<ITagRepository, Repositories.V1.TagRepository, Repositories.V2.TagRepository>(services);
            RegisteredServices<IArticleTagRepository, Repositories.V1.ArticleTagRepository, Repositories.V2.ArticleTagRepository>(services);

            services.AddScoped<IContentsUnitOfWork, ContentsUnitOfWork>();

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
