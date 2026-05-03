using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class DirectHealthDamageEvent : HealthDamageEvent
{
    internal DirectHealthDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, DamageResult result) : base(evtcItem, agentData, skillData)
    {
        HealthDamage = evtcItem.Value;
        AgainstDowned = evtcItem.IsOffcycle == 1;
        IsAbsorbed = result == DamageResult.DirectOrBuffAbsorb || result == DamageResult.DirectOrBuffInvert;
        IsBlind = result == DamageResult.DirectBlind;
        IsBlocked = result == DamageResult.DirectBlock;
        HasCrit = result == DamageResult.DirectCrit;
        IsEvaded = result == DamageResult.DirectEvade;
        HasGlanced = result == DamageResult.DirectGlance;
        ShieldDamage = evtcItem.IsShields > 0 ? (int)evtcItem.OverstackValue : 0;
        HasHit = result == DamageResult.DirectNormal || HasGlanced || HasCrit; 
    }

    internal override void MakeIntoAbsorbed()
    {
        HasHit = false;
        HasCrit = false;
        HasGlanced = false;
        IsBlind = false;
        IsBlocked = false;
        IsEvaded = false;
        IsAbsorbed = true;

        HealthDamage = 0;
        ShieldDamage = 0;
    }
}
