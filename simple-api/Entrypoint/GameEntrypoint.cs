using Microsoft.AspNetCore.Mvc;
using simple_api.Controllers.dto;
using simple_api.Core.Strategy;

namespace simple_api.Controllers;

[ApiController]
[Route("/api/v1/games")]
public class GameEntrypoint : ControllerBase{

    private readonly ConsoleStrategyFactory _factory;

    public GameEntrypoint(ConsoleStrategyFactory factory)
    {
        _factory = factory;
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
        var strategy = _factory.Resolve(consoleType);
        if (strategy == null)
        {
            return NotFound(new DataResponse<GameResponse>(data: null, message: "Console não suportado", statusCode: 404));
        }

        var game = strategy.Execute();
        return Ok(new DataResponse<GameResponse>(data: game, message: "Sucesso", statusCode: 200));
    }
    
}