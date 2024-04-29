using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var resourceBuilder = ResourceBuilder.CreateDefault()
        .AddService(serviceName: "weather-forecast",
            serviceNamespace: "demo",
            serviceVersion: "0.0.0",
            autoGenerateServiceInstanceId: true);

builder.Services.AddOpenTelemetry()
    .WithMetrics(builder =>
    {
        builder.SetResourceBuilder(resourceBuilder);
        builder.AddAspNetCoreInstrumentation();
        builder.AddConsoleExporter();
        builder.AddProcessInstrumentation();
        builder.AddRuntimeInstrumentation();
        builder.AddOtlpExporter();
    })
    .WithTracing(builder =>
    {
        builder.SetResourceBuilder(resourceBuilder);
        builder.AddAspNetCoreInstrumentation();
        builder.AddConsoleExporter();
        builder.AddOtlpExporter();
    });
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.SetResourceBuilder(resourceBuilder);
    logging.AddProcessor(new CustomLogProcessor());
    logging.AddConsoleExporter();
    logging.AddOtlpExporter();
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/error", () =>
{
    app.Logger.LogError("This is an error");
    return "Error logged";
});

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

class CustomLogProcessor : BaseProcessor<LogRecord>
{
    public override void OnEnd(LogRecord data)
    {
        if (data.LogLevel == LogLevel.Error)
        {
            Activity.Current?.SetStatus(ActivityStatusCode.Error);
            Activity.Current?.AddEvent(new ActivityEvent(data.Body ?? string.Empty));
        }

        base.OnEnd(data);
    }
}
