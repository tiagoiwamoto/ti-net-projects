// csharp
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class ExternalApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<ExternalApiClient> _logger;

    public ExternalApiClient(IHttpClientFactory factory, ILogger<ExternalApiClient> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    // apiName deve ser "apiA" ou "apiB" (nomes usados no Program.cs)
    public async Task<string> GetDataAsync(string id, string apiName)
    {
        var client = _factory.CreateClient(apiName);
        var res = await client.GetAsync($"/api/people/{id}");
        if (!res.IsSuccessStatusCode)
        {
            _logger.LogWarning("Request to {Api} returned {Status}", apiName, res.StatusCode);
        }
        return await res.Content.ReadAsStringAsync();
    }
}