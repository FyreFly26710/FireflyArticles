using FF.Articles.Backend.Identity.API.Infrastructure;
using FF.Articles.Backend.Identity.API.Interfaces;
using FF.Articles.Backend.Identity.API.MappingProfiles;
using FF.Articles.Backend.Identity.API.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

builder.Services.AddDbContext<IdentityDbContext>();// to do : add connection string

builder.Services.AddAutoMapper(typeof(UserMappingProfile));
builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin() // Allow any origin
                   .AllowAnyMethod() // Allow any HTTP method
                   .AllowAnyHeader(); // Allow any header
        });
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure this is set for production (HTTPS)
        options.Cookie.SameSite = SameSiteMode.Strict; // Secure cookie behavior
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
