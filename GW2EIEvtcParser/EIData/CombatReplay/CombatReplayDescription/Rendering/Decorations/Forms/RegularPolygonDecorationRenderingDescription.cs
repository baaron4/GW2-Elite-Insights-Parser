using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.RegularPolygonDecoration;

namespace GW2EIEvtcParser.EIData;

public class RegularPolygonDecorationRenderingDescription : FormDecorationRenderingDescription
{

    internal RegularPolygonDecorationRenderingDescription(ParsedEvtcLog log, RegularPolygonDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = Types.RegularPolygon;
    }
}
