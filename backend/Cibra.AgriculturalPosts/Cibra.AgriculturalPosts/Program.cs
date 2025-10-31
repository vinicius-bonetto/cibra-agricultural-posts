using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Cibra.AgriculturalPosts.Application.Commands;
using Cibra.AgriculturalPosts.Application.Queries;
using Cibra.AgriculturalPosts.Domain.Interfaces;
using Cibra.AgriculturalPosts.Infrastructure.Data;
using Cibra.AgriculturalPosts.Infrastructure.Repositories;
using Cibra.AgriculturalPosts.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Database - SQL Server
var sqlConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(sqlConnection, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    })
);

// Repositories
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// AI Service
builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAI"));
builder.Services.AddHttpClient<IAIService, OpenAIService>();

// Command and Query Handlers
builder.Services.AddScoped<CreatePostCommandHandler>();
builder.Services.AddScoped<UpdatePostCommandHandler>();
builder.Services.AddScoped<ProcessAIMentionCommandHandler>();
builder.Services.AddScoped<GetPostByIdQueryHandler>();
builder.Services.AddScoped<GetAllPostsQueryHandler>();
builder.Services.AddScoped<GetUserPostsQueryHandler>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? new[] { "http://localhost:3000" }
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database");

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cibra Agricultural Posts API",
        Version = "v1",
        Description = "API for managing agricultural posts with AI-powered analysis",
        Contact = new OpenApiContact
        {
            Name = "Cibra Development Team",
            Email = "dev@cibra.com"
        }
    });

    c.EnableAnnotations();
});

var app = builder.Build();

// Apply migrations automatically (for development)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        await db.Database.MigrateAsync();
        Console.WriteLine("✅ Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error applying migrations: {ex.Message}");
    }
}

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cibra Agricultural Posts API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

// Only use HTTPS redirection in production or when HTTPS is properly configured
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Redirect("/swagger"));

// Startup message
Console.WriteLine("🌱 Cibra Agricultural Posts API");
Console.WriteLine($"🚀 Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"🔗 Swagger UI: {(app.Environment.IsDevelopment() ? "http://localhost:5000" : "N/A")}");

app.Run();