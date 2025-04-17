using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.ProgressBarDecoration;

namespace GW2EIEvtcParser.EIData;

public class ProgressBarDecorationRenderingDescription : RectangleDecorationRenderingDescription
{

    public readonly IReadOnlyList<(long, double)> Progress;
    public int InterpolationMethod { get; private set; }
    internal ProgressBarDecorationRenderingDescription(ParsedEvtcLog log, ProgressBarDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = Types.ProgressBar;
        Progress = decoration.Progress;
        InterpolationMethod = (int)decoration.Method;
    }
}
