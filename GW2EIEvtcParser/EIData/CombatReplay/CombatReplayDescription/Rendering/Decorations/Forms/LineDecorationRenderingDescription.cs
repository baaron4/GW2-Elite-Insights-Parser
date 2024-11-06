using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.LineDecoration;

namespace GW2EIEvtcParser.EIData;

public class LineDecorationRenderingDescription : FormDecorationRenderingDescription
{
    public readonly object ConnectedFrom;

    internal LineDecorationRenderingDescription(ParsedEvtcLog log, LineDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = "Line";
        ConnectedFrom = decoration.ConnectedFrom.GetConnectedTo(map, log);
    }
}
