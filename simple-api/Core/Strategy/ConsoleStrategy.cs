using simple_api.Controllers.dto;

namespace simple_api.Core.Strategy;

public interface ConsoleStrategy
{
    GameResponse Execute();
    Boolean supports(string consoleType);
}