using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;
using Newtonsoft.Json;

namespace GW2EIBuilders.JsonModels
{
    /// <summary>
    /// Class corresponding to a rotation
    /// </summary>
    public class JsonRotation
    {
        /// <summary>
        /// Class corresponding to a skill
        /// </summary>
        public class JsonSkill
        {
            [JsonProperty]
            /// <summary>
            /// Time at which the skill was cast
            /// </summary>
            public int CastTime { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Duration of the animation, instant cast if 0
            /// </summary>
            public int Duration { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Gained time from the animation, could be negative, which means time was lost
            /// </summary>
            public int TimeGained { get; internal set; }
            [JsonProperty]
            /// <summary>	
            /// Value between -1 (100% slow) and 1 (100% quickness) \n
            /// Prior arcdps activation update (nov 07 2019) this value can only be 0 or 1
            /// </summary>	
            public double Quickness { get; internal set; }

            [JsonConstructor]
            internal JsonSkill()
            {

            }

            internal JsonSkill(AbstractCastEvent cl)
            {
                CastTime = (int)cl.Time;
                Duration = cl.ActualDuration;
                TimeGained = cl.SavedDuration;
                Quickness = cl.Acceleration;
            }
        }

        [JsonProperty]
        /// <summary>
        /// ID of the skill
        /// </summary>
        /// <seealso cref="JsonLog.SkillMap"/>
        public long Id { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// List of casted skills
        /// </summary>
        /// <seealso cref="JsonSkill"/>
        public IReadOnlyList<JsonSkill> Skills { get; internal set; }

        [JsonConstructor]
        internal JsonRotation()
        {

        }

        protected JsonRotation(ParsedEvtcLog log, long skillID, List<AbstractCastEvent> skillCasts, Dictionary<string, JsonLog.SkillDesc> skillDesc)
        {
            if (!skillDesc.ContainsKey("s" + skillID))
            {
                SkillItem skill = skillCasts.First().Skill;
                skillDesc["s" + skillID] = new JsonLog.SkillDesc(skill, log.LogData.GW2Build, log.SkillData);
            }
            Id = skillID;
            Skills = skillCasts.Select(x => new JsonSkill(x)).ToList();
        }

        internal static List<JsonRotation> BuildJsonRotationList(ParsedEvtcLog log, Dictionary<long, List<AbstractCastEvent>> skillByID, Dictionary<string, JsonLog.SkillDesc> skillDesc)
        {
            var res = new List<JsonRotation>();
            foreach (KeyValuePair<long, List<AbstractCastEvent>> pair in skillByID)
            {
                res.Add(new JsonRotation(log, pair.Key, pair.Value, skillDesc));
            }
            return res;
        }
    }
}
