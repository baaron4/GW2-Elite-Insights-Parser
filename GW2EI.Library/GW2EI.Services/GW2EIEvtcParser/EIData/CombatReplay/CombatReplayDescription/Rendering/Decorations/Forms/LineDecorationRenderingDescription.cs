using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.LineDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class LineDecorationRenderingDescription : FormDecorationRenderingDescription
{
    public readonly ConnectorDescription ConnectedFrom;

    internal LineDecorationRenderingDescription(ParsedEvtcLog log, LineDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature, StringToUlongHandler stringToUlongHandler) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature, stringToUlongHandler)
    {
        Type = Types.Line;
        ConnectedFrom = decoration.ConnectedFrom.GetConnectedTo(map, log);
    }
}
