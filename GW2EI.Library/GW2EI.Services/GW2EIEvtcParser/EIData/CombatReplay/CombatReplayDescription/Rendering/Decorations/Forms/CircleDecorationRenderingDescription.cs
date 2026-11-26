using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.CircleDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class CircleDecorationRenderingDescription : FormDecorationRenderingDescription
{

    internal CircleDecorationRenderingDescription(ParsedEvtcLog log, CircleDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature, StringToUlongHandler stringToUlongHandler) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature, stringToUlongHandler)
    {
        Type = Types.Circle;
    }
}
