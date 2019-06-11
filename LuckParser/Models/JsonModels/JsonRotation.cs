using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    /// <summary>
    /// Class corresponding to a skill
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
            public int CastTime;
            /// <summary>
            /// Duration of the animation
            /// </summary>
            public int Duration;
            /// <summary>
            /// Gained time from the animation, could be negative, which means time was lost
            /// </summary>
            public int TimeGained;
            /// <summary>
            /// Animation started while under quickness
            /// </summary>
            public bool Quickness;

            public JsonSkill(AbstractCastEvent cl)
            {
                int timeGained = 0;
                if (cl.ReducedAnimation && cl.ActualDuration < cl.ExpectedDuration)
                {
                    timeGained = cl.ExpectedDuration - cl.ActualDuration;
                }
                else if (cl.Interrupted)
                {
                    timeGained = -cl.ActualDuration;
                }
                CastTime = (int)cl.Time;
                Duration = cl.ActualDuration;
                TimeGained = timeGained;
                Quickness = cl.UnderQuickness;
            }
        }

        /// <summary>
        /// ID of the skill
        /// </summary>
        /// <seealso cref="JsonLog.SkillMap"/>
        public long Id;
        /// <summary>
        /// List of casted skills
        /// </summary>
        /// <seealso cref="JsonSkill"/>
        public List<JsonSkill> Skills;
    }
}
