using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using static GW2EIBuilders.JsonModels.JsonLogBuilder;
using static GW2EIJSON.JsonRotation;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities;

/// <summary>
/// Class corresponding to a rotation
/// </summary>
internal static class JsonRotationBuilder
{
    private static JsonSkill BuildJsonSkill(ParsedEvtcLog log, CastEvent cl, UsedJsonData usedData)
    {
        var (LastCastBarExecute, ControlReturn) = usedData.GetComputedSkillDuration(log, cl.SkillID, cl.TimeScale, false);
        var jsonSkill = new JsonSkill
        {
            CastTime = (int)cl.Time,
            Duration = cl.ActualDuration,
            TimeGained = cl.SavedDuration,
            Quickness = cl.AccelerationQuicknessSlow,
            ExpectedCastBarDuration = LastCastBarExecute ?? null,
            ExpectedReturnControlDuration = ControlReturn ?? null,
        };
        if (cl.IgnoreOnRotationRender())
        {
            jsonSkill.IgnoreOnRotationRender = true;
        }
        return jsonSkill;
    }
    private static JsonRotation BuildJsonRotation(ParsedEvtcLog log, long skillID, List<CastEvent> skillCasts, UsedJsonData usedData)
    {
        var jsonRotation = new JsonRotation();
        usedData.skills.TryAdd(skillID, skillCasts.First().Skill);
        jsonRotation.Id = skillID;
        jsonRotation.Skills = skillCasts.Select(x => BuildJsonSkill(log, x, usedData)).ToList();
        return jsonRotation;
    }

    public static IEnumerable<JsonRotation> BuildJsonRotationList(ParsedEvtcLog log, IEnumerable<IGrouping<long, CastEvent>> skillByID, UsedJsonData usedData)
    {
        return skillByID.Select(group => BuildJsonRotation(log, group.Key, group.ToList(), usedData));
    }
}
