using API.Middleware;
using Application;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
});

// Add services to the container
builder.Services.AddControllers();

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configure OpenAPI
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "Sports Match API",
            Version = "v1",
            Description = "API for connecting people who want to play sports together",
            Contact = new()
            {
                Name = "Support",
                Email = "support@sportsmatch.com"
            }
        };
        return Task.CompletedTask;
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    options.AddPolicy("Production", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "https://yourdomain.com" };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

// Configure JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Sports Match API")
            .WithTheme(ScalarTheme.Purple)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .WithPreferredScheme("Bearer")
            .WithApiKeyAuthentication(x => x.Token = "");
    });

    // Use permissive CORS in development
    app.UseCors("AllowAll");
}
else
{
    // Use strict CORS in production
    app.UseCors("Production");

    // Use HTTPS redirection in production
    app.UseHttpsRedirection();
}

// Global error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// Serilog request logging
app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

// Database initialization
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        // Apply migrations
        //logger.LogInformation("Applying database migrations...");
        //await context.Database.MigrateAsync();
        //logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database");
        throw;
    }
}

Log.Information("Starting Sports Match API...");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}