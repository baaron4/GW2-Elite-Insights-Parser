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
        private readonly ParseEnum.PhysicalResult _result;

        public DirectDamageEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            Damage = evtcItem.Value;
            _result = ParseEnum.GetPhysicalResult(evtcItem.Result);
            IsIndirectDamage = false;
            IsCondi = false;
        }

        public override bool IsAbsorb()
        {
            return _result == ParseEnum.PhysicalResult.Absorb;
        }

        public override bool IsBlind()
        {
            return _result == ParseEnum.PhysicalResult.Blind;
        }

        public override bool IsBlock()
        {
            return _result == ParseEnum.PhysicalResult.Block;
        }

        public override bool IsCrit()
        {
            return _result == ParseEnum.PhysicalResult.Crit;
        }

        public override bool IsDowned()
        {
            return _result == ParseEnum.PhysicalResult.Downed;
        }

        public override bool IsEvade()
        {
            return _result == ParseEnum.PhysicalResult.Evade;
        }

        public override bool IsGlance()
        {
            return _result == ParseEnum.PhysicalResult.Glance;
        }

        public override bool IsHit()
        {
            return _result == ParseEnum.PhysicalResult.Normal || _result == ParseEnum.PhysicalResult.Crit || _result == ParseEnum.PhysicalResult.Glance || _result == ParseEnum.PhysicalResult.KillingBlow; //Downed and Interrupt omitted for now due to double procing mechanics || _result == ParseEnum._result.Downed || _result == ParseEnum._result.Interrupt;
        }

        public override bool IsInterrupt()
        {
            return _result == ParseEnum.PhysicalResult.Interrupt;
        }

        public override bool IsKilled()
        {
            return _result == ParseEnum.PhysicalResult.KillingBlow;
        }
    }
}
