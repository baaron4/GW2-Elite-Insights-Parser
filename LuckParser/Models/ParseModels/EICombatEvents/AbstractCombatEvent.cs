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
#if DEBUG
        protected CombatItem OriginalCombatEvent;
#endif

        protected AbstractCombatEvent(long logTime, long offset)
        {
            Time = logTime - offset;
        }

        public void OverrideTime(long time)
        {
            Time = time;
        }
    }
}
