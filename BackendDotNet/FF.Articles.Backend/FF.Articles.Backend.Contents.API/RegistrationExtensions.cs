using FF.Articles.Backend.Contents.API.Repositories;
using FF.Articles.Backend.Contents.API.Services;
using FF.Articles.Backend.Contents.API.Services.RemoteServices;
using FF.Articles.Backend.Contents.API.Validators.Articles;
using FluentValidation.AspNetCore;
using Nest;

namespace FF.Articles.Backend.Contents.API
{
    public static class RegistrationExtensions
    {
        public static IServiceCollection AddContentsServices(this IServiceCollection services)
        {

            // Add HttpClient for IdentityRemoteService
            services.AddHttpClient<IIdentityRemoteService, IdentityRemoteService>();

            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ITopicService, TopicService>();

            services.AddScoped<IContentsUnitOfWork, ContentsUnitOfWork>();

            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ITopicRepository, TopicRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IArticleTagRepository, ArticleTagRepository>();

            return services;
        }

        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IElasticClient>(sp =>
            {
                var elasticUri = configuration.GetConnectionString("elasticsearch") ?? "http://localhost:9200";
                var settings = new ConnectionSettings(new Uri(elasticUri))
                    .DefaultIndex("articles");

                return new ElasticClient(settings);
            });
            services.AddScoped<IElasticSearchArticleService, ESArticleService>();
            services.AddScoped<ElasticsearchSyncService>();
        }
        public static void AddFluentValidation(this IServiceCollection services)
        {
            // This will add the FluentValidation middleware to the pipeline
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<ArticleAddRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<ArticleEditRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<ArticleQueryRequestValidator>();
        }
    }
}
