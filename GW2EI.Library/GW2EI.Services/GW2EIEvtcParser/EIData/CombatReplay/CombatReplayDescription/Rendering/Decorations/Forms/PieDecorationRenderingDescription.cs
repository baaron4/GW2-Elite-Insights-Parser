using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.PieDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class PieDecorationRenderingDescription : CircleDecorationRenderingDescription
{
    internal PieDecorationRenderingDescription(ParsedEvtcLog log, PieDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature, StringToUlongHandler stringToUlongHandler) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature, stringToUlongHandler)
    {
        Type = Types.Pie;
    }

}
