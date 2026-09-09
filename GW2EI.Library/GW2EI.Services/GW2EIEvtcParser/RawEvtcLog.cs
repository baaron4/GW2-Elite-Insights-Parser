using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIGW2API;

namespace GW2EIEvtcParser;

public class RawEvtcLog
{
    public readonly LogMetadata LogMetadata;
    public readonly AgentData AgentData;
    public readonly SkillData SkillData;
    public readonly IReadOnlyList<CombatItem> CombatItems;
    public readonly CombatData CombatData;
    public readonly EvtcParserSettings ParserSettings;

    private readonly ParserController _operation;

    internal RawEvtcLog(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, SkillData skillData,
            IReadOnlyList<CombatItem> combatItems, IReadOnlyList<Player> playerList, IReadOnlyDictionary<uint, ExtensionHandler> extensions, EvtcParserSettings parserSettings, GW2APIController apiController, ParserController operation)
    {
        AgentData = agentData;
        SkillData = skillData;
        ParserSettings = parserSettings;
        _operation = operation;

        CombatItems = combatItems;

        _operation.UpdateProgressWithCancellationCheck("Parsing: Creating GW2EI Combat Events");
        CombatData = new CombatData(combatItems, logData, AgentData, SkillData, playerList, operation, extensions, evtcVersion, parserSettings, apiController);

        _operation.UpdateProgressWithCancellationCheck("Parsing: Creating GW2EI Log Meta Data");
        LogMetadata = new LogMetadata(evtcVersion, CombatData, logData.EvtcLogEnd - logData.EvtcLogStart, playerList, extensions, operation);
    }

    public void UpdateProgressWithCancellationCheck(string status)
    {
        _operation.UpdateProgressWithCancellationCheck(status);
    }
}
