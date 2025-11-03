using System.Text.Json.Serialization;
using simple_api.Core.Strategy;
using Newtonsoft.Json.Serialization;
using simple_api.adapter;
using simple_api.Config;
using Polly;
using Polly.Extensions.Http;
using simple_api.Core.Usecase;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };
    });
builder.Services.AddEndpointsApiExplorer();

// Register strategy implementations and factory
builder.Services.AddTransient<ConsoleStrategy, NintendoStrategy>();
builder.Services.AddTransient<ConsoleStrategy, SonyStrategy>();

// registra o usecase que o controller precisa
builder.Services.AddScoped<GameUsecase>();

// Registrar typed HttpClient com políticas Polly
builder.Services.AddHttpClient("apiA", client =>
    {
        client.BaseAddress = new Uri("https://swapi.dev");
    })
    .AddPolicyHandler((sp, request) => HttpPolicyConfig.GetPolicy(sp.GetService<ILogger<ExternalApiClient>>()));

builder.Services.AddHttpClient("apiB", client =>
    {
        client.BaseAddress = new Uri("https://api-b.example.com");
    })
    .AddPolicyHandler((sp, request) => HttpPolicyConfig.GetPolicy(sp.GetService<ILogger<ExternalApiClient>>()));

// Registra o client que usará IHttpClientFactory
builder.Services.AddTransient<ExternalApiClient>();

var app = builder.Build();
app.UseRequestInterceptor();
app.UseMiddleware<MdcMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();


app.Run();