using FF.Articles.Backend.Identity.API.Infrastructure;
using FF.Articles.Backend.Identity.API.MappingProfiles;
using FF.Articles.Backend.Identity.API.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using FF.Articles.Backend.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);


builder.AddServiceDefaults();
//builder.AddEFSqlServer<IdentityDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

//builder.Logging.AddConsole();

builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging();
});

builder.Services.AddAutoMapper(typeof(UserMappingProfile));

builder.Services.AddScoped<IUserService, UserService>();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowedOrigins",
//        builder =>
//        {
//            builder.AllowAnyHeader()
//                   .AllowAnyMethod()
//                   //.AllowAnyOrigin()
//                   .WithOrigins("http://localhost:22000")
//                   .AllowCredentials();
//        });
//});
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.Cookie.Name = "AuthCookie";
//        //options.ExpireTimeSpan = TimeSpan.FromDays(7); // optional
//        options.Cookie.HttpOnly = true;
//        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // only for localhost
//        options.Cookie.SameSite = SameSiteMode.Lax; // Allows cross-API requests on localhost
//        //options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure this is set for production (HTTPS)
//        //options.Cookie.SameSite = SameSiteMode.Lax; // Secure cookie behavior
//    });
//builder.Services.AddDataProtection()
//    .PersistKeysToFileSystem(new DirectoryInfo(@"D:\202502"))
//    .SetApplicationName("SharedAuth");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseCors("AllowedOrigins");
//app.UseAuthentication();
//app.UseAuthorization();

app.AddCookieAuthMiddleware();

app.MapControllers();

app.Run();
