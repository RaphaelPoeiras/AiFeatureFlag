using System.Text;
using AiFeatureFlags.Api.Middleware;
using AiFeatureFlags.Application.Abstractions;
using AiFeatureFlags.Application.Auth;
using AiFeatureFlags.Application.FeatureFlags;
using AiFeatureFlags.Infrastructure.Data;
using AiFeatureFlags.Infrastructure.Dev;
using AiFeatureFlags.Infrastructure.Repositories;
using AiFeatureFlags.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AI Feature Flags API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste JWT as: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var sqliteConnectionString = builder.Configuration.GetConnectionString("Sqlite")
                           ?? "Data Source=./data/app.db";

builder.Services.AddSingleton<IDbConnectionFactory>(_ => new SqliteConnectionFactory(sqliteConnectionString));
builder.Services.AddSingleton<SqliteDatabaseMigrator>();

builder.Services.AddScoped<IUserAccountRepository, SqlUserAccountRepository>();
builder.Services.AddScoped<IFeatureFlagRepository, SqlFeatureFlagRepository>();

builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
                  ?? throw new InvalidOperationException("Jwt configuration section is missing.");
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<IJwtTokenIssuer, JwtTokenIssuer>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FeatureFlagService>();
builder.Services.AddScoped<DemoDataSeeder>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey))
        };
    });

builder.Services.AddAuthorization();

var frontendOrigin = builder.Configuration["Cors:FrontendOrigin"] ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "data"));

await using (var scope = app.Services.CreateAsyncScope())
{
    var migrator = scope.ServiceProvider.GetRequiredService<SqliteDatabaseMigrator>();
    await migrator.MigrateAsync(CancellationToken.None).ConfigureAwait(false);

    if (app.Configuration.GetValue("SeedDemoData", false))
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DemoDataSeeder>();
        await seeder.SeedAsync(CancellationToken.None).ConfigureAwait(false);
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("Health");

app.Run();

public partial class Program;
