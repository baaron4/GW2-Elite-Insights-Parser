using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

public class OutgoingDamageModifier : DamageModifier
{
    internal OutgoingDamageModifier(DamageModifierDescriptor damageModDescriptor) : base(damageModDescriptor, damageModDescriptor.DmgSrc)
    {
        ID = Name.GetHashCode();
        Incoming = false;
    }

    public override int GetTotalDamage(AbstractSingleActor actor, ParsedEvtcLog log, AbstractSingleActor? t, long start, long end)
    {
        FinalDPS damageData = actor.GetDPSStats(t, log, start, end);
        return (CompareType) switch
        {
            DamageType.All                => DmgSrc == DamageSource.All ? damageData.Damage : damageData.ActorDamage,
            DamageType.Condition          => DmgSrc == DamageSource.All ? damageData.CondiDamage : damageData.ActorCondiDamage,
            DamageType.Power              => DmgSrc == DamageSource.All ? damageData.PowerDamage : damageData.ActorPowerDamage,
            DamageType.LifeLeech          => DmgSrc == DamageSource.All ? damageData.LifeLeechDamage : damageData.ActorLifeLeechDamage,
            DamageType.Strike             => DmgSrc == DamageSource.All ? damageData.StrikeDamage : damageData.ActorStrikeDamage,
            DamageType.StrikeAndCondition => DmgSrc == DamageSource.All ? damageData.StrikeDamage + damageData.CondiDamage : damageData.ActorStrikeDamage + damageData.ActorCondiDamage,
            DamageType.StrikeAndConditionAndLifeLeech => DmgSrc == DamageSource.All ? damageData.StrikeDamage + damageData.CondiDamage + damageData.LifeLeechDamage : damageData.ActorStrikeDamage + damageData.ActorCondiDamage + damageData.ActorLifeLeechDamage,
            _ => throw new NotImplementedException("Not implemented damage type " + CompareType),
        };
    }

    public override IEnumerable<AbstractHealthDamageEvent> GetHitDamageEvents(AbstractSingleActor actor, ParsedEvtcLog log, AbstractSingleActor? t, long start, long end)
    {
        return DmgSrc == DamageSource.All ? actor.GetHitDamageEvents(t, log, start, end, SrcType) : actor.GetJustActorHitDamageEvents(t, log, start, end, SrcType);
    }
    internal override AgentItem GetFoe(AbstractHealthDamageEvent evt)
    {
        return evt.To;
    }
}
