﻿using Asp.Versioning;
using FF.Articles.Backend.ServiceDefaults.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FF.Articles.Backend.ServiceDefaults
{
    public static class ApiVersioningExtensions
    {
        public static WebApplicationBuilder AddCustomApiVersioning(this WebApplicationBuilder builder)
        {
            builder.Services.AddApiVersioning(x =>
            {
                x.DefaultApiVersion = new ApiVersion(1.0);
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.ApiVersionReader = new HeaderApiVersionReader("api-version");
            }).AddMvc().AddApiExplorer();

            return builder;
        }
        public static WebApplicationBuilder AddApiVersioningSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            builder.Services.AddSwaggerGen(c =>
            {
                c.CustomOperationIds(apiDesc =>
                {
                    if (apiDesc.ActionDescriptor is ControllerActionDescriptor descriptor)
                    {
                        var resource = descriptor.ControllerName.Replace("Controller", "");
                        return $"api{resource}{descriptor.ActionName}";
                    }
                    return null;
                });
            });


            return builder;
        }
        public static WebApplication UseCustomSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                foreach (var description in app.DescribeApiVersions())
                {
                    x.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName);
                }
            });


            return app;
        }
    }

}
