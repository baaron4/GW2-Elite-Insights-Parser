using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class MechanicEvent : AbstractCombatEvent
    {
        public AgentItem Src { get; }
        public Mechanic Mechanic { get; }

        public MechanicEvent(long time, Mechanic mech, AgentItem src) : base(time, 0)
        {
            Src = src;
            Mechanic = mech;
        }
    }
}
