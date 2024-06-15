using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using static GW2EIJSON.JsonRotation;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities
{
    /// <summary>
    /// Class corresponding to a rotation
    /// </summary>
    internal static class JsonRotationBuilder
    {
        private static JsonSkill BuildJsonSkill(AbstractCastEvent cl)
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
        private static JsonRotation BuildJsonRotation(ParsedEvtcLog log, long skillID, List<AbstractCastEvent> skillCasts, Dictionary<long, SkillItem> skillMap)
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

        public static List<JsonRotation> BuildJsonRotationList(ParsedEvtcLog log, Dictionary<long, List<AbstractCastEvent>> skillByID, Dictionary<long, SkillItem> skillMap)
        {
            var res = new List<JsonRotation>();
            foreach (KeyValuePair<long, List<AbstractCastEvent>> pair in skillByID)
            {
                res.Add(BuildJsonRotation(log, pair.Key, pair.Value, skillMap));
            }
            return res;
        }
    }
}
