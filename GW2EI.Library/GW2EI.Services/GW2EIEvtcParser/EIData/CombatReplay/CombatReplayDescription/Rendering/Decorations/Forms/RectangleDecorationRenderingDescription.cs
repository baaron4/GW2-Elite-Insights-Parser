using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.RectangleDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class RectangleDecorationRenderingDescription : FormDecorationRenderingDescription
{

    internal RectangleDecorationRenderingDescription(ParsedEvtcLog log, RectangleDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature, StringToUlongHandler stringToUlongHandler) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature, stringToUlongHandler)
    {
        Type = Types.Rectangle;
    }
}
