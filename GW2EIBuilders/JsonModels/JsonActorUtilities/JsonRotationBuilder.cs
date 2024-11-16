using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using static GW2EIJSON.JsonRotation;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities;

/// <summary>
/// Class corresponding to a rotation
/// </summary>
internal static class JsonRotationBuilder
{
    private static JsonSkill BuildJsonSkill(CastEvent cl)
    {
        var jsonSkill = new JsonSkill
        {
            CastTime = (int)cl.Time,
            Duration = cl.ActualDuration,
            TimeGained = cl.SavedDuration,
            Quickness = cl.Acceleration
        };
        return jsonSkill;
    }
    private static JsonRotation BuildJsonRotation(ParsedEvtcLog log, long skillID, List<CastEvent> skillCasts, Dictionary<long, SkillItem> skillMap)
    {
        var jsonRotation = new JsonRotation();
        if (!skillMap.ContainsKey(skillID))
        {
            SkillItem skill = skillCasts.First().Skill;
            skillMap[skillID] = skill;
        }
        jsonRotation.Id = skillID;
        jsonRotation.Skills = skillCasts.Select(x => BuildJsonSkill(x)).ToList();
        return jsonRotation;
    }

    public static IEnumerable<JsonRotation> BuildJsonRotationList(ParsedEvtcLog log, IEnumerable<IGrouping<long, CastEvent>> skillByID, Dictionary<long, SkillItem> skillMap)
    {
        return skillByID.Select(group => BuildJsonRotation(log, group.Key, group.ToList(), skillMap));
    }
}
