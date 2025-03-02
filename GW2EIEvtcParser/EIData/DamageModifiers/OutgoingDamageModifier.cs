using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

public class OutgoingDamageModifier : DamageModifier
{
    internal OutgoingDamageModifier(DamageModifierDescriptor damageModDescriptor) : base(damageModDescriptor, damageModDescriptor.DmgSrc)
    {
        Incoming = false;
    }

    public override int GetTotalDamage(SingleActor actor, ParsedEvtcLog log, SingleActor? t, long start, long end)
    {
        DamageStatistics damageData = actor.GetDamageStats(t, log, start, end);
        return (CompareType) switch
        {
            DamageType.All                => DmgSrc == DamageSource.All ? damageData.Damage : damageData.ActorDamage,
            DamageType.Condition          => DmgSrc == DamageSource.All ? damageData.ConditionDamage : damageData.ActorConditionDamage,
            DamageType.Power              => DmgSrc == DamageSource.All ? damageData.PowerDamage : damageData.ActorPowerDamage,
            DamageType.LifeLeech          => DmgSrc == DamageSource.All ? damageData.LifeLeechDamage : damageData.ActorLifeLeechDamage,
            DamageType.Strike             => DmgSrc == DamageSource.All ? damageData.StrikeDamage : damageData.ActorStrikeDamage,
            DamageType.StrikeAndCondition => DmgSrc == DamageSource.All ? damageData.StrikeDamage + damageData.ConditionDamage : damageData.ActorStrikeDamage + damageData.ActorConditionDamage,
            DamageType.StrikeAndConditionAndLifeLeech => DmgSrc == DamageSource.All ? damageData.StrikeDamage + damageData.ConditionDamage + damageData.LifeLeechDamage : damageData.ActorStrikeDamage + damageData.ActorConditionDamage + damageData.ActorLifeLeechDamage,
            _ => throw new NotImplementedException("Not implemented damage type " + CompareType),
        };
    }

    public override IEnumerable<HealthDamageEvent> GetHitDamageEvents(SingleActor actor, ParsedEvtcLog log, SingleActor? t, long start, long end)
    {
        return DmgSrc == DamageSource.All ? actor.GetHitDamageEvents(t, log, start, end, SrcType) : actor.GetJustActorHitDamageEvents(t, log, start, end, SrcType);
    }
    internal override AgentItem GetFoe(HealthDamageEvent evt)
    {
        return evt.To;
    }
}
