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
        public DirectDamageEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            Damage = evtcItem.Value;
            ParseEnum.PhysicalResult result = ParseEnum.GetPhysicalResult(evtcItem.Result);
            IsCondi = false;
            IsAbsorb = result == ParseEnum.PhysicalResult.Absorb;
            IsBlind = result == ParseEnum.PhysicalResult.Blind;
            IsBlock = result == ParseEnum.PhysicalResult.Block;
            IsCrit = result == ParseEnum.PhysicalResult.Crit;
            IsDowned = result == ParseEnum.PhysicalResult.Downed;
            IsEvade = result == ParseEnum.PhysicalResult.Evade;
            IsGlance = result == ParseEnum.PhysicalResult.Glance;
            IsHit = result == ParseEnum.PhysicalResult.Normal || result == ParseEnum.PhysicalResult.Crit || result == ParseEnum.PhysicalResult.Glance || result == ParseEnum.PhysicalResult.KillingBlow; //Downed and Interrupt omitted for now due to double procing mechanics || result == ParseEnum.PhysicalResult.Downed || result == ParseEnum.PhysicalResult.Interrupt;
            IsKilled = result == ParseEnum.PhysicalResult.KillingBlow;
            IsInterrupt = result == ParseEnum.PhysicalResult.Interrupt;
        }
    }
}
