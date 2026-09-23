using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser;

public abstract class EvtcLog
{
    public LogMetadata LogMetadata { get; protected set; }
    public readonly AgentData AgentData;
    public readonly SkillData SkillData;
    public readonly IReadOnlyList<CombatItem> CombatItems = [];
    public CombatData CombatData { get; protected set; }
    public readonly EvtcParserSettings ParserSettings;

    protected readonly ParserController _operation;

    internal EvtcLog(AgentData agentData, SkillData skillData,
            IReadOnlyList<CombatItem> combatItems, EvtcParserSettings parserSettings, ParserController operation)
    {
        AgentData = agentData;
        SkillData = skillData;
        ParserSettings = parserSettings;
        _operation = operation;

        CombatItems = combatItems;
    }

    public void UpdateProgressWithCancellationCheck(string status)
    {
        _operation.UpdateProgressWithCancellationCheck(status);
    }
}
