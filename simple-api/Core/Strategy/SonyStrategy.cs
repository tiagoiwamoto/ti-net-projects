using simple_api.Controllers.dto;

namespace simple_api.Core.Strategy;

public class SonyStrategy : ConsoleStrategy
{
    public GameResponse Execute()
    {
        return new GameResponse("Gran Turismo", "n/a", "Playstation", 80.5, "sony");
    }

    public bool supports(string consoleType)
    {
        return consoleType.Equals("PlayStation", StringComparison.OrdinalIgnoreCase);
    }
}