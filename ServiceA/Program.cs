using Prometheus;
using RateLimiter;
using RateLimiter.Middleware;
using RateLimiter.Stores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddSingleton<IRateLimitCounterStore, InMemoryRateLimitCounterStore>();
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowPolicy("ServiceA", fixedWindowOptions =>
    {
        fixedWindowOptions.PermitLimit = 20;
        fixedWindowOptions.Window = TimeSpan.FromSeconds(1);
        fixedWindowOptions.KeyGenerator = context => context.Request.Headers["User-Identity"].ToString() ??  $"anonymous";
    }, true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMetricServer();
app.UseHttpMetrics();

app.UseMiddleware<RateLimitingMiddleware>();

app.MapControllers();

app.UseHttpsRedirection();
app.Run();