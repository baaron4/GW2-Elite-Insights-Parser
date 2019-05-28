using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class AbstractCombatEvent
    {
        public long Time { get; protected set; }

        protected AbstractCombatEvent(CombatItem evtcItem)
        {
            Time = evtcItem.Time;
        }

        public void OverrideTime(long time)
        {
            Time = time;
        }
    }
}
