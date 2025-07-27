using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class NoDamageHealthDamageEvent : HealthDamageEvent
{
    internal NoDamageHealthDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, PhysicalResult result) : base(evtcItem, agentData, skillData)
    {
        HasDowned = result == PhysicalResult.Downed;
        HasKilled = result == PhysicalResult.KillingBlow;
        HasInterrupted = result == PhysicalResult.Interrupt;
        IsNotADamageEvent = true;
    }

    internal override void MakeIntoAbsorbed()
    {
        // Nothing to do
    }
}
