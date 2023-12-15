using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class OutgoingDamageModifier : DamageModifier
    {
        internal OutgoingDamageModifier(DamageModifierDescriptor damageModDescriptor) : base(damageModDescriptor, damageModDescriptor.DmgSrc)
        {
            ID = Name.GetHashCode();
            Incoming = false;
        }

        public override int GetTotalDamage(AbstractSingleActor actor, ParsedEvtcLog log, AbstractSingleActor t, long start, long end)
        {
            FinalDPS damageData = actor.GetDPSStats(t, log, start, end);
            switch (CompareType)
            {
                case DamageType.All:
                    return DmgSrc == DamageSource.All ? damageData.Damage : damageData.ActorDamage;
                case DamageType.Condition:
                    return DmgSrc == DamageSource.All ? damageData.CondiDamage : damageData.ActorCondiDamage;
                case DamageType.Power:
                    return DmgSrc == DamageSource.All ? damageData.PowerDamage : damageData.ActorPowerDamage;
                case DamageType.LifeLeech:
                    return DmgSrc == DamageSource.All ? damageData.LifeLeechDamage : damageData.ActorLifeLeechDamage;
                case DamageType.Strike:
                    return DmgSrc == DamageSource.All ? damageData.StrikeDamage : damageData.ActorStrikeDamage;
                case DamageType.StrikeAndCondition:
                    return DmgSrc == DamageSource.All ? damageData.StrikeDamage + damageData.CondiDamage : damageData.ActorStrikeDamage + damageData.ActorCondiDamage;
                case DamageType.StrikeAndConditionAndLifeLeech:
                    return DmgSrc == DamageSource.All ? damageData.StrikeDamage + damageData.CondiDamage + damageData.LifeLeechDamage : damageData.ActorStrikeDamage + damageData.ActorCondiDamage + damageData.ActorLifeLeechDamage;
                default:
                    throw new NotImplementedException("Not implemented damage type " + CompareType);
            }
        }

        public override IReadOnlyList<AbstractHealthDamageEvent> GetHitDamageEvents(AbstractSingleActor actor, ParsedEvtcLog log, AbstractSingleActor t, long start, long end)
        {
            return DmgSrc == DamageSource.All ? actor.GetHitDamageEvents(t, log, start, end, SrcType) : actor.GetJustActorHitDamageEvents(t, log, start, end, SrcType);
        }
        internal override AgentItem GetFoe(AbstractHealthDamageEvent evt)
        {
            return evt.To;
        }
    }
}
