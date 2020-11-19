namespace GW2EIEvtcParser.ParsedData
{
    public class DirectHealthDamageEvent : AbstractHealthDamageEvent
    {
        internal DirectHealthDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, ArcDPSEnums.PhysicalResult result) : base(evtcItem, agentData, skillData)
        {
            Damage = evtcItem.Value;
            IsAbsorbed = result == ArcDPSEnums.PhysicalResult.Absorb;
            IsBlind = result == ArcDPSEnums.PhysicalResult.Blind;
            IsBlocked = result == ArcDPSEnums.PhysicalResult.Block;
            HasCrit = result == ArcDPSEnums.PhysicalResult.Crit;
            HasDowned = result == ArcDPSEnums.PhysicalResult.Downed;
            IsEvaded = result == ArcDPSEnums.PhysicalResult.Evade;
            HasGlanced = result == ArcDPSEnums.PhysicalResult.Glance;        
            HasKilled = result == ArcDPSEnums.PhysicalResult.KillingBlow;
            HasInterrupted = result == ArcDPSEnums.PhysicalResult.Interrupt;
            ShieldDamage = evtcItem.IsShields > 0 ? (int)evtcItem.OverstackValue : 0;
            HasHit = result == ArcDPSEnums.PhysicalResult.Normal || HasGlanced || HasCrit || HasKilled; //Downed and Interrupt omitted for now due to double procing mechanics || result == ParseEnum.PhysicalResult.Downed || result == ParseEnum.PhysicalResult.Interrupt;
            DoubleProcHit = HasDowned || HasInterrupted;
        }

        public override bool IsCondi(ParsedEvtcLog log)
        {
            return false;
        }
    }
}
