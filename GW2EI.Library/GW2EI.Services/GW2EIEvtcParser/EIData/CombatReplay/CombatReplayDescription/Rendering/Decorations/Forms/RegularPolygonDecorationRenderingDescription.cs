using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.RegularPolygonDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class RegularPolygonDecorationRenderingDescription : FormDecorationRenderingDescription
{

    internal RegularPolygonDecorationRenderingDescription(ParsedEvtcLog log, RegularPolygonDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature, StringToUlongHandler stringToUlongHandler) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature, stringToUlongHandler)
    {
        Type = Types.RegularPolygon;
    }
}
