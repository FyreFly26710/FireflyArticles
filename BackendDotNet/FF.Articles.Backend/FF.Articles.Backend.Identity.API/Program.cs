using FF.Articles.Backend.Common.Extensions;
using FF.Articles.Backend.Common.Middlewares;
using FF.Articles.Backend.Identity.API.Repositories;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.AddServiceDefaults();

var connectionString = configuration.GetConnectionString("identitydb") ?? throw new Exception("identitydb connection string not found");
builder.AddNpgsql<IdentityDbContext>(connectionString);
builder.Services.AddHttpClient();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOAuthService, OAuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddMigration<IdentityDbContext, IdentityDbContextSeed>();
}

builder.Services.AddControllers();

builder.AddBasicApi();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseBasicSwagger();

app.UseHttpsRedirection();

app.AddCookieAuthMiddleware();

app.MapControllers();

app.Run();
