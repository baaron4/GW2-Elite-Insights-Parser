using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonSkill
    {
        public int Time;
        public int Duration;
        public int TimeGained;
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
            Time = (int)cl.Time;
            Duration = cl.ActualDuration;
            TimeGained = timeGained;
            Quickness = cl.StartActivation == Parser.ParseEnum.Activation.Quickness;
        }
    }
}
