using simple_api.Controllers.dto;

namespace simple_api.Core.Strategy;

public class NintendoStrategy : ConsoleStrategy
{
    public GameResponse Execute()
    {
        return new GameResponse("Mario Kart", "n/a", "Switch", 80.5, "nintendo");
    }

    public bool supports(string consoleType)
    {
        return consoleType.Equals("Nintendo", StringComparison.OrdinalIgnoreCase);
    }
}