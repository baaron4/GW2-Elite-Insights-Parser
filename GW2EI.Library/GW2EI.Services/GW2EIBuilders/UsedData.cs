
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParserHelpers;

internal abstract class BaseUsedData
{
    private readonly Dictionary<(long Skill, double TimeScale, bool Water), (int? LastCastBarExecute, int? ControlReturn)> skillDurations = new(32);

    public (int? LastCastBarExecute, int? ControlReturn) GetComputedSkillDuration(ParsedEvtcLog log, long skillID, double timeScale, bool water)
    {
        if (log.LogMetadata.EvtcBuild < ArcDPSEnums.ArcDPSBuilds.SkillInfoRework)
        {
            return (null, null);
        }
        timeScale = TimeScale.NormalizeCommonScales(timeScale);
        var key = (skillID, timeScale, water);
        if (!skillDurations.TryGetValue(key, out var duration))
        {
            var animation = log.SkillData.Get(skillID).SkillInfo?.GetAnimation(water);
            var scaling = TimeScale.GetTickScaling(timeScale);
            duration.LastCastBarExecute = animation?.ComputeCastBarExecute(true, scaling);
            duration.ControlReturn = animation?.ComputeControlReturn(scaling);
            skillDurations.Add(key, duration);
        }
        return duration;
    }
};
