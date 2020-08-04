using GW2EIUtils;

namespace GW2EIEvtcParser.ParsedData
{
    public class DirectDamageEvent : AbstractDamageEvent
    {
        public DirectDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            Damage = evtcItem.Value;
            ArcDPSEnums.PhysicalResult result = ArcDPSEnums.GetPhysicalResult(evtcItem.Result);
            IsAbsorbed = result == ArcDPSEnums.PhysicalResult.Absorb;
            IsBlind = result == ArcDPSEnums.PhysicalResult.Blind;
            IsBlocked = result == ArcDPSEnums.PhysicalResult.Block;
            HasCrit = result == ArcDPSEnums.PhysicalResult.Crit;
            HasDowned = result == ArcDPSEnums.PhysicalResult.Downed;
            IsEvaded = result == ArcDPSEnums.PhysicalResult.Evade;
            HasGlanced = result == ArcDPSEnums.PhysicalResult.Glance;
            HasHit = result == ArcDPSEnums.PhysicalResult.Normal || result == ArcDPSEnums.PhysicalResult.Crit || result == ArcDPSEnums.PhysicalResult.Glance || result == ArcDPSEnums.PhysicalResult.KillingBlow; //Downed and Interrupt omitted for now due to double procing mechanics || result == ParseEnum.PhysicalResult.Downed || result == ParseEnum.PhysicalResult.Interrupt;
            HasKilled = result == ArcDPSEnums.PhysicalResult.KillingBlow;
            HasInterrupted = result == ArcDPSEnums.PhysicalResult.Interrupt;
            ShieldDamage = evtcItem.IsShields > 0 ? (int)evtcItem.OverstackValue : 0;
        }

        public override bool IsCondi(ParsedEvtcLog log)
        {
            return false;
        }
    }
}
