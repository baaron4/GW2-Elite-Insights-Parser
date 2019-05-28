using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class DirectDamageEvent : AbstractDamageEvent
    {
        public ParseEnum.PhysicalResult Result { get; }

        public DirectDamageEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            Result = ParseEnum.GetPhysicalResult(evtcItem.Result);
            Damage = evtcItem.Value;
        }

    }
}
