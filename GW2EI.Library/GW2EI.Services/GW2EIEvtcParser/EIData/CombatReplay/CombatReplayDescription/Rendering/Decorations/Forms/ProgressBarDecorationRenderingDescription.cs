using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.ProgressBarDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class ProgressBarDecorationRenderingDescription : RectangleDecorationRenderingDescription
{

    public readonly IReadOnlyList<double[]> Progress;
    public int InterpolationMethod { get; private set; }
    internal ProgressBarDecorationRenderingDescription(ParsedEvtcLog log, ProgressBarDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature, StringToUlongHandler stringToUlongHandler) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature, stringToUlongHandler)
    {
        Type = Types.ProgressBar;
        Progress = decoration.Progress.Select(x => new double[] { x.Item1, x.Item2 }).ToList();
        InterpolationMethod = (int)decoration.Method;
    }
}
