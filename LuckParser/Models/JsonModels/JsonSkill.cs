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

        public JsonSkill(ParseModels.CastLog cl)
        {
            int timeGained = 0;
            if (cl.EndActivation == Parser.ParseEnum.Activation.CancelFire && cl.ActualDuration < cl.ExpectedDuration)
            {
                timeGained = cl.ExpectedDuration - cl.ActualDuration;
            }
            else if (cl.EndActivation == Parser.ParseEnum.Activation.CancelCancel)
            {
                timeGained = -cl.ActualDuration;
            }
            CastTime = (int)cl.Time;
            Duration = cl.ActualDuration;
            TimeGained = timeGained;
            Quickness = cl.StartActivation == Parser.ParseEnum.Activation.Quickness;
        }
    }
}
