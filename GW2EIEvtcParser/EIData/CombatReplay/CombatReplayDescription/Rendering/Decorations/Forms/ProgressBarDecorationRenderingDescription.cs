using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.ProgressBarDecoration;

namespace GW2EIEvtcParser.EIData;

public class ProgressBarDecorationRenderingDescription : RectangleDecorationRenderingDescription
{

    public readonly IReadOnlyList<(long, double)> Progress;
    internal ProgressBarDecorationRenderingDescription(ParsedEvtcLog log, ProgressBarDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = "ProgressBar";
        Progress = decoration.Progress;
    }
}
