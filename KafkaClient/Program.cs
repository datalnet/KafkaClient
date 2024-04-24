using KafkaClient;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var model = new TerminalDataModel()
{
    MessageProccessor = "TerminalData"
};

var model1 = new TargetDefinitionModel()
{
    MessageProccessor = "TargetDefinition"
};

//builder.Services.AddScoped<TestKafkaClientCoreService>();
//builder.Services.AddScoped<TestAKafkaClientCoreService>();


builder.Services.AddKafkaClient<TestKafkaClientCoreService>(options =>
{
    options.UseModel(model);
});

builder.Services.AddKafkaClient<TestAKafkaClientCoreService>(options =>
{
    options.UseModel(model1);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (IServiceProvider serviceProvider) =>
{
    var TT = serviceProvider.GetService<TestKafkaClientCoreService>();
    //var YY = serviceProvider.GetService<KafkaClientOptions>();
    //var YY = serviceProvider.GetService<TestKafkaClientCoreService>();

    //var service = testKafkaClientCoreService;

    //var serviceA = testAKafkaClientCoreService;

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
