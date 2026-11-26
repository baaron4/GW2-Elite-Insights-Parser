using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.FormDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public abstract class FormDecorationRenderingDescription : AttachedDecorationRenderingDescription
{
    public readonly bool Fill;
    public readonly long GrowingEnd;
    public readonly bool GrowingReverse;

    internal FormDecorationRenderingDescription(ParsedEvtcLog log, FormDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature, StringToUlongHandler stringToUlongHandler) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature, stringToUlongHandler)
    {
        Fill = decoration.Filled;
        GrowingEnd = decoration.GrowingEnd;
        GrowingReverse = decoration.GrowingReverse;
    }

}
