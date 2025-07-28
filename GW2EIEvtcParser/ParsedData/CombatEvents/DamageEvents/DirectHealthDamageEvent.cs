using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class DirectHealthDamageEvent : HealthDamageEvent
{
    internal DirectHealthDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, PhysicalResult result) : base(evtcItem, agentData, skillData)
    {
        HealthDamage = evtcItem.Value;
        AgainstDowned = evtcItem.IsOffcycle == 1;
        IsAbsorbed = result == PhysicalResult.Absorb;
        IsBlind = result == PhysicalResult.Blind;
        IsBlocked = result == PhysicalResult.Block;
        HasCrit = result == PhysicalResult.Crit;
        IsEvaded = result == PhysicalResult.Evade;
        HasGlanced = result == PhysicalResult.Glance;
        ShieldDamage = evtcItem.IsShields > 0 ? (int)evtcItem.OverstackValue : 0;
        HasHit = result == PhysicalResult.Normal || HasGlanced || HasCrit; 
    }

    internal override void MakeIntoAbsorbed()
    {
        HasHit = false;
        HasCrit = false;
        HasGlanced = false;
        IsAbsorbed = true;
        HealthDamage = 0;
        ShieldDamage = 0;
    }
}
