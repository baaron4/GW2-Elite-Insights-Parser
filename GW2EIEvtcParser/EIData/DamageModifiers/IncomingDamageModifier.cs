using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

public class IncomingDamageModifier : DamageModifier
{
    internal IncomingDamageModifier(DamageModifierDescriptor damageModDescriptor) : base(damageModDescriptor)
    {
        if (damageModDescriptor.DmgSrc != DamageModifiersUtils.DamageSource.Incoming)
        {
            throw new InvalidDataException("IncomingDamageModifier must have Incoming Damage Source type");
        }
        Incoming = true;
    }

    public override int GetTotalDamage(SingleActor actor, ParsedEvtcLog log, SingleActor? t, long start, long end)
    {
        DefensePerTargetStatistics defenseData = actor.GetDefenseStats(t, log, start, end);
        return (CompareType) switch
        {
            DamageType.All                            => defenseData.DamageTaken,
            DamageType.Condition                      => defenseData.ConditionDamageTaken,
            DamageType.Power                          => defenseData.PowerDamageTaken,
            DamageType.LifeLeech                      => defenseData.LifeLeechDamageTaken,
            DamageType.Strike                         => defenseData.StrikeDamageTaken,
            DamageType.StrikeAndCondition             => defenseData.StrikeDamageTaken + defenseData.ConditionDamageTaken,
            DamageType.StrikeAndLifeLeech             => defenseData.StrikeDamageTaken + defenseData.LifeLeechDamageTaken,
            DamageType.ConditionAndLifeLeech          => defenseData.ConditionDamageTaken + defenseData.LifeLeechDamageTaken,
            DamageType.StrikeAndConditionAndLifeLeech => defenseData.StrikeDamageTaken + defenseData.ConditionDamageTaken + defenseData.LifeLeechDamageTaken,
            _ => throw new NotImplementedException("Not implemented damage type " + CompareType),
        };
    }

    public override IReadOnlyList<HealthDamageEvent> GetHitDamageEvents(SingleActor actor, ParsedEvtcLog log, SingleActor? t, long start, long end)
    {
        return actor.GetHitDamageTakenEvents(t, log, start, end, SrcType);
    }
    internal override AgentItem GetFoe(HealthDamageEvent evt)
    {
        return evt.From;
    }

    internal override AgentItem GetActor(HealthDamageEvent evt)
    {
        return evt.To;
    }
}
