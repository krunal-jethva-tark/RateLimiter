using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using RateLimiter;
using RateLimiter.Middleware;
using RateLimiter.Stores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Adds telemetry support
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
    .WithMetrics(metrics 
        => metrics
            .AddMeter(RateLimitingMetrics.MeterName)
            .AddPrometheusExporter()
    );

builder.Services.AddSingleton<IRateLimitCounterStore, InMemoryRateLimitCounterStore>();
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowPolicy("ServiceC", fixedWindowOptions =>
    {
        fixedWindowOptions.PermitLimit = 20;
        fixedWindowOptions.Window = TimeSpan.FromSeconds(1);
    }).MarkAsDefault();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the Prometheus scraping endpoint
app.MapPrometheusScrapingEndpoint();

app.UseMiddleware<RateLimitingMiddleware>();

app.MapControllers();

app.UseHttpsRedirection();
app.Run();