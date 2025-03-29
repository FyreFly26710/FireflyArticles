using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.ServiceDefaults;
public static class ProgramExtensions
{
    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder, IConfiguration configuration)
    {

        builder.AddLogger();
        builder.AddCors(configuration);
        builder.AddCookieAuth();
        builder.Services.AddHttpClient();

        return builder;
    }

    public static WebApplicationBuilder AddLogger(this WebApplicationBuilder builder)
    {
        builder.Logging.AddConsole();
        return builder;
    }
    //public static WebApplicationBuilder AddEFSqlServer<TContext>(this WebApplicationBuilder builder, string connectionString) 
    //    where TContext:DbContext
    //{
    //    builder.Services.AddDbContext<TContext>(options =>
    //    {
    //        options.UseSqlServer(connectionString)
    //        .LogTo(Console.WriteLine, LogLevel.Information)
    //        .EnableSensitiveDataLogging();
    //    });
    //    return builder;
    //}



}
