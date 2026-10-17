using System.Collections.Generic;
using GW2EIParserCommons;

namespace GW2EIParserAvalonia;

public class CommandLineOptions
{
    public IReadOnlyList<string> LogFiles { get; init; } = [];
    public string? ConfigPath { get; init; }

    public ProgramHelper.InspectionMode InspectMode { get; init; }
}
