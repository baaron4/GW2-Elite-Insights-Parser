using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.FormDecoration;

namespace GW2EIEvtcParser.EIData;

public abstract class FormDecorationRenderingDescription : AttachedDecorationRenderingDescription
{
    public readonly bool Fill;
    public readonly long GrowingEnd;
    public readonly bool GrowingReverse;

    internal FormDecorationRenderingDescription(ParsedEvtcLog log, FormDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Fill = decoration.Filled;
        GrowingEnd = decoration.GrowingEnd;
        GrowingReverse = decoration.GrowingReverse;
    }

}
