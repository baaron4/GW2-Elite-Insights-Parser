using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DoughnutDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class DoughnutDecorationRenderingDescription : FormDecorationRenderingDescription
{

    internal DoughnutDecorationRenderingDescription(ParsedEvtcLog log, DoughnutDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature, StringToUlongHandler stringToUlongHandler) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature, stringToUlongHandler)
    {
        Type = Types.Doughnut;
    }

}
