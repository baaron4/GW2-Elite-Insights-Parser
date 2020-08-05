using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIParser.Builders.JsonModels
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
            /// <summary>
            /// Time at which the skill was cast
            /// </summary>
            public int CastTime { get; }
            /// <summary>
            /// Duration of the animation
            /// </summary>
            public int Duration { get; }
            /// <summary>
            /// Gained time from the animation, could be negative, which means time was lost
            /// </summary>
            public int TimeGained { get; }
            /// <summary>	
            /// Value between -1 (100% slow) and 1 (100% quickness) \n
            /// Prior arcdps activation update (nov 07 2019) this value can only be 0 or 1
            /// </summary>	
            public double Quickness { get; }

            internal JsonSkill(AbstractCastEvent cl)
            {
                CastTime = (int)cl.Time;
                Duration = cl.ActualDuration;
                TimeGained = cl.SavedDuration;
                Quickness = cl.Acceleration;
            }
        }

        /// <summary>
        /// ID of the skill
        /// </summary>
        /// <seealso cref="JsonLog.SkillMap"/>
        public long Id { get; }
        /// <summary>
        /// List of casted skills
        /// </summary>
        /// <seealso cref="JsonSkill"/>
        public List<JsonSkill> Skills { get; }


        protected JsonRotation(long skillID, List<AbstractCastEvent> skillCasts, Dictionary<string, JsonLog.SkillDesc> skillDesc)
        {
            if (!skillDesc.ContainsKey("s" + skillID))
            {
                SkillItem skill = skillCasts.First().Skill;
                skillDesc["s" + skillID] = new JsonLog.SkillDesc(skill);
            }
            Id = skillID;
            Skills = skillCasts.Select(x => new JsonSkill(x)).ToList();
        }

        internal static List<JsonRotation> BuildJsonRotationList(Dictionary<long, List<AbstractCastEvent>> skillByID, Dictionary<string, JsonLog.SkillDesc> skillDesc)
        {
            var res = new List<JsonRotation>();
            foreach (KeyValuePair<long, List<AbstractCastEvent>> pair in skillByID)
            {
                res.Add(new JsonRotation(pair.Key, pair.Value, skillDesc));
            }
            return res;
        }
    }
}
