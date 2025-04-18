using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.OverheadProgressBarDecoration;

namespace GW2EIEvtcParser.EIData;

public class OverheadProgressBarDecorationRenderingDescription : ProgressBarDecorationRenderingDescription
{

    internal OverheadProgressBarDecorationRenderingDescription(ParsedEvtcLog log, OverheadProgressBarDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = Types.ProgressBarOverhead;
    }
}
