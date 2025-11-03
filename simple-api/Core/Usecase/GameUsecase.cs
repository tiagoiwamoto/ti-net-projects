using System.Collections;

namespace simple_api.Core.Usecase;

public class GameUsecase
{
    private readonly ILogger<GameUsecase> _logger;
    private readonly ExternalApiClient _externalApiClient;

    public GameUsecase(ILogger<GameUsecase> logger, ExternalApiClient externalApiClient)
    {
        _logger = logger;
        _externalApiClient = externalApiClient;
    }
    
// csharp
    public async Task<List<string>> GetGameDataAsync(string id, string apiName)
    {
        if (string.IsNullOrWhiteSpace(apiName))
            throw new ArgumentException("apiName não pode ser nulo ou vazio", nameof(apiName));

        var firstId = string.IsNullOrWhiteSpace(id) ? "1" : id;
        var secondId = "2";

        var personOneTask = _externalApiClient.GetDataAsync(firstId, apiName);
        var personTwoTask = _externalApiClient.GetDataAsync(secondId, apiName);

        try
        {
            await Task.WhenAll(personOneTask, personTwoTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao chamar API externa ({Api}) para ids {Id1} e {Id2}", apiName, firstId, secondId);
            throw;
        }

        var personOne = personOneTask.Result ?? string.Empty;
        var personTwo = personTwoTask.Result ?? string.Empty;

        var timestamp1 = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        _logger.LogInformation("Dados da API externa 1 ({Timestamp}): {PersonOne}", timestamp1, personOne);

        var timestamp2 = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        _logger.LogInformation("Dados da API externa 2 ({Timestamp}): {PersonTwo}", timestamp2, personTwo);

        return new List<string> { personOne, personTwo };
    }

}