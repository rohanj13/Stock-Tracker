using Portfolio.Api.Endpoints;
using Portfolio.Api.Extensions;
using Portfolio.Application.Extensions;
using Portfolio.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add AWS Lambda support for ASP.NET Core
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// Add services to the container
builder.Services.AddApiCors(builder.Configuration);
builder.Services.AddApiHealthChecks();

// Register application layer services
builder.Services.AddApplication();

// Register infrastructure layer services
builder.Services.AddInfrastructure(builder.Configuration);

// Add API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Portfolio Diversification API",
        Version = "v1",
        Description = "API for portfolio diversification and investment exposure analytics",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Portfolio Team"
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Portfolio API v1");
    });
}

// Use API middleware (includes exception handling, CORS, logging, etc.)
app.UseApiMiddleware();

// Map health check endpoints
app.MapHealthChecks("/healthz");
app.MapHealthEndpoints();

// Map API endpoints
var apiGroup = app.MapGroup("/api")
    .WithOpenApi();

apiGroup.MapUserEndpoints();
apiGroup.MapPortfolioEndpoints();
apiGroup.MapStockEndpoints();
apiGroup.MapEtfEndpoints();
apiGroup.MapExposureEndpoints();

app.Run();
