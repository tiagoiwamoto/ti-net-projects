// ...existing code...

namespace simple_api.Core.Strategy;

using System.Collections.Generic;
using simple_api.Controllers.dto;

public class ConsoleStrategyFactory
{
    private readonly IEnumerable<ConsoleStrategy> _strategies;

    public ConsoleStrategyFactory(IEnumerable<ConsoleStrategy> strategies)
    {
        _strategies = strategies;
    }

    // Resolve a strategy that supports the given consoleType.
    // Returns null if no strategy supports the consoleType.
    public ConsoleStrategy? Resolve(string consoleType)
    {
        foreach (var s in _strategies)
        {
            if (s.supports(consoleType)) return s;
        }
        return null;
    }
}

