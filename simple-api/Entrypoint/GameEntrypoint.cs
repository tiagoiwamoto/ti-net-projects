using Microsoft.AspNetCore.Mvc;
using simple_api.adapter;
using simple_api.Controllers.dto;
using simple_api.Core.Strategy;
using simple_api.Core.Usecase;

namespace simple_api.Controllers;

[ApiController]
[Route("/api/v1/games")]
public class GameEntrypoint : ControllerBase{

    private readonly ILogger<GameEntrypoint> _logger;
    private readonly IEnumerable<ConsoleStrategy> _strategies;
    private readonly GameUsecase _gameUsecase;
    

    public GameEntrypoint(ILogger<GameEntrypoint> logger,IEnumerable<ConsoleStrategy> strategies, GameUsecase gameUsecase)
    {
        _logger = logger;
        _strategies = strategies;
        _gameUsecase = gameUsecase;
    }
    
    [HttpGet(Name = "RecuperarGames")]
    public DataResponse<List<string>> listarGames() {
        DataResponse<List<string>> response = new DataResponse<List<string>>(
            data: new List<string>() { "Game1", "Game2", "Game3" },
            message: "Sucesso",
            statusCode: 200
        );
        return response;
    }

    // Example: GET /api/v1/games/by-console?console=Nintendo
    [HttpGet("by-console")]
    public ActionResult<DataResponse<GameResponse>> GetByConsole([FromQuery(Name = "console")] string consoleType)
    {
        _logger.LogInformation("GetByConsole called with console={ConsoleType}", consoleType);
        var strategy = _strategies.First(s => s.supports(consoleType));
        if (strategy == null)
        {
            return NotFound(new DataResponse<GameResponse>(data: null, message: "Console não suportado", statusCode: 404));
        }

        var game = strategy.Execute();
        return Ok(new DataResponse<GameResponse>(data: game, message: "Sucesso", statusCode: 200));
    }
    
    // Example: GET /api/v1/games/by-console/Nintendo
    [HttpGet("by-console/{console}")]
    public ActionResult<DataResponse<GameResponse>> GetByConsolePath([FromRoute(Name = "console")] string consoleType)
    {
        _logger.LogInformation("GetByConsole called with console={ConsoleType}", consoleType);
        var strategy = _strategies.FirstOrDefault(s => s.supports(consoleType));
        if (strategy == null)
        {
            return NotFound(new DataResponse<GameResponse>(data: null, message: "Console não suportado", statusCode: 404));
        }

        var game = strategy.Execute();
        return Ok(new DataResponse<GameResponse>(data: game, message: "Sucesso", statusCode: 200));
    }
    
    [HttpPost(Name = "CadastrarGame")]
    public ActionResult<DataResponse<GameResponse>> CadastrarGame([FromBody] GameRequest request)
    {
        var response = new GameResponse(
            nome: request.nome,
            descricao: request.descricao,
            image: request.image,
            consoleType: request.consoleType,
            preco: request.preco
        );
        var agencia = Mdc.Get("agencia");
        _logger.LogInformation("Agencia no cadastro de game: {Agencia}", agencia);
        
        var games = _gameUsecase.GetGameDataAsync("some-id", "apiA").Result;
        _logger.LogInformation("Dados obtidos no cadastro de game: {Games}", games);
        
        return Created(new Uri("api/v1/games/by-console/" + request.consoleType, UriKind.Relative),
            new DataResponse<GameResponse>(
                data: response,
                message: "Game cadastrado com sucesso",
                statusCode: 201
            )
        );
    }
    
    
    
}