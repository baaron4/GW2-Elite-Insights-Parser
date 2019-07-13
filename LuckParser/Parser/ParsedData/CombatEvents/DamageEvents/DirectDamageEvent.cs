namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class DirectDamageEvent : AbstractDamageEvent
    {
        public DirectDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, agentData, skillData, offset)
        {
            Damage = evtcItem.Value;
            ParseEnum.PhysicalResult result = ParseEnum.GetPhysicalResult(evtcItem.Result);
            IsAbsorbed = result == ParseEnum.PhysicalResult.Absorb;
            IsBlind = result == ParseEnum.PhysicalResult.Blind;
            IsBlocked = result == ParseEnum.PhysicalResult.Block;
            HasCrit = result == ParseEnum.PhysicalResult.Crit;
            HasDowned = result == ParseEnum.PhysicalResult.Downed;
            IsEvaded = result == ParseEnum.PhysicalResult.Evade;
            HasGlanced = result == ParseEnum.PhysicalResult.Glance;
            HasHit = result == ParseEnum.PhysicalResult.Normal || result == ParseEnum.PhysicalResult.Crit || result == ParseEnum.PhysicalResult.Glance || result == ParseEnum.PhysicalResult.KillingBlow; //Downed and Interrupt omitted for now due to double procing mechanics || result == ParseEnum.PhysicalResult.Downed || result == ParseEnum.PhysicalResult.Interrupt;
            HasKilled = result == ParseEnum.PhysicalResult.KillingBlow;
            HasInterrupted = result == ParseEnum.PhysicalResult.Interrupt;
            ShieldDamage = evtcItem.IsShields > 0 ? (int)evtcItem.OverstackValue : 0;
        }

        public override bool IsCondi(ParsedLog log)
        {
            return false;
        }
    }
}
