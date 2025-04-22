using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

public class OutgoingDamageModifier : DamageModifier
{
    internal OutgoingDamageModifier(DamageModifierDescriptor damageModDescriptor) : base(damageModDescriptor)
    {
        if (damageModDescriptor.DmgSrc == DamageSource.Incoming)
        {
            throw new InvalidDataException("OutgoingDamageModifier must not have Incoming Damage Source type");
        }
        Incoming = false;
    }

    public override int GetTotalDamage(SingleActor actor, ParsedEvtcLog log, SingleActor? t, long start, long end)
    {
        DamageStatistics damageData = actor.GetDamageStats(t, log, start, end);
        switch (CompareType)
        {
            case DamageType.All:
                switch (DmgSrc)
                {
                    case DamageSource.All:
                    case DamageSource.PetsOnly:
                        return damageData.Damage;
                    case DamageSource.NoPets:
                        return damageData.ActorDamage;
                    /*case DamageSource.PetsOnly:
                        return damageData.Damage - damageData.ActorDamage;*/
                }
                break;
            case DamageType.Condition:
                switch (DmgSrc)
                {
                    case DamageSource.All:
                    case DamageSource.PetsOnly:
                        return damageData.ConditionDamage;
                    case DamageSource.NoPets:
                        return damageData.ActorConditionDamage;
                    /*case DamageSource.PetsOnly:
                        return damageData.ConditionDamage - damageData.ActorConditionDamage;*/
                }
                break;
            case DamageType.Power:
                switch (DmgSrc)
                {
                    case DamageSource.All:
                    case DamageSource.PetsOnly:
                        return damageData.PowerDamage;
                    case DamageSource.NoPets:
                        return damageData.ActorPowerDamage;
                    /*case DamageSource.PetsOnly:
                        return damageData.PowerDamage - damageData.ActorPowerDamage;*/
                }
                break;
            case DamageType.LifeLeech:
                switch (DmgSrc)
                {
                    case DamageSource.All:
                    case DamageSource.PetsOnly:
                        return damageData.LifeLeechDamage;
                    case DamageSource.NoPets:
                        return damageData.ActorLifeLeechDamage;
                    /*case DamageSource.PetsOnly:
                        return damageData.LifeLeechDamage - damageData.ActorLifeLeechDamage;*/
                }
                break;
            case DamageType.Strike:
                switch (DmgSrc)
                {
                    case DamageSource.All:
                    case DamageSource.PetsOnly:
                        return damageData.StrikeDamage;
                    case DamageSource.NoPets:
                        return damageData.ActorStrikeDamage;
                    /*case DamageSource.PetsOnly:
                        return damageData.StrikeDamage - damageData.ActorStrikeDamage;*/
                }
                break;
            case DamageType.StrikeAndCondition:
                switch (DmgSrc)
                {
                    case DamageSource.All:
                    case DamageSource.PetsOnly:
                        return damageData.StrikeDamage + damageData.ConditionDamage;
                    case DamageSource.NoPets:
                        return damageData.ActorStrikeDamage + damageData.ActorConditionDamage;
                    /*case DamageSource.PetsOnly:
                        return (damageData.StrikeDamage + damageData.ConditionDamage) - (damageData.ActorStrikeDamage + damageData.ActorConditionDamage);*/
                }
                break;
            case DamageType.StrikeAndLifeLeech:
                switch (DmgSrc)
                {
                    case DamageSource.All:
                    case DamageSource.PetsOnly:
                        return damageData.StrikeDamage + damageData.LifeLeechDamage;
                    case DamageSource.NoPets:
                        return damageData.ActorStrikeDamage + damageData.ActorLifeLeechDamage;
                    /*case DamageSource.PetsOnly:
                        return (damageData.StrikeDamage + damageData.LifeLeechDamage) - (damageData.ActorStrikeDamage + damageData.ActorLifeLeechDamage);*/
                }
                break;
            case DamageType.ConditionAndLifeLeech:
                switch (DmgSrc)
                {
                    case DamageSource.All:
                    case DamageSource.PetsOnly:
                        return damageData.ConditionDamage + damageData.LifeLeechDamage;
                    case DamageSource.NoPets:
                        return damageData.ActorConditionDamage + damageData.ActorLifeLeechDamage;
                    /*case DamageSource.PetsOnly:
                        return (damageData.ConditionDamage + damageData.LifeLeechDamage) - (damageData.ActorConditionDamage + damageData.ActorLifeLeechDamage);*/
                }
                break;
            case DamageType.StrikeAndConditionAndLifeLeech:
                switch (DmgSrc)
                {
                    case DamageSource.All:
                    case DamageSource.PetsOnly:
                        return damageData.StrikeDamage + damageData.ConditionDamage + damageData.LifeLeechDamage;
                    case DamageSource.NoPets:
                        return damageData.ActorStrikeDamage + damageData.ActorConditionDamage + damageData.ActorLifeLeechDamage;
                    /*case DamageSource.PetsOnly:
                        return (damageData.StrikeDamage + damageData.ConditionDamage + damageData.LifeLeechDamage) - (damageData.ActorStrikeDamage + damageData.ActorConditionDamage + damageData.ActorLifeLeechDamage);*/
                }
                break;
            default:
                throw new NotImplementedException("Not implemented damage type " + CompareType);
        }
        throw new NotImplementedException("Not implemented damage source " + DmgSrc);
    }

    public override IEnumerable<HealthDamageEvent> GetHitDamageEvents(SingleActor actor, ParsedEvtcLog log, SingleActor? t, long start, long end)
    {
        switch (DmgSrc)
        {
            case DamageSource.All:
                return actor.GetHitDamageEvents(t, log, start, end, SrcType);
            case DamageSource.NoPets:
                return actor.GetJustActorHitDamageEvents(t, log, start, end, SrcType);
            case DamageSource.PetsOnly:
                return actor.GetJustMinionsHitDamageEvents(t, log, start, end, SrcType);
        }
        throw new NotImplementedException("Not implemented damage source " + DmgSrc);
    }
    internal override AgentItem GetFoe(HealthDamageEvent evt)
    {
        return evt.To;
    }
}
