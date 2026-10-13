using System.Collections.Generic;

namespace GW2EIParserAvalonia;

public class CommandLineOptions
{
    public IReadOnlyList<string> LogFiles { get; init; } = [];
    public string? ConfigPath { get; init; }

    public bool InspectOnly { get; init; }
}
