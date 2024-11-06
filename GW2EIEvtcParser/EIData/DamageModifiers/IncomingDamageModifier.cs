using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

public class IncomingDamageModifier : DamageModifier
{
    internal IncomingDamageModifier(DamageModifierDescriptor damageModDescriptor) : base(damageModDescriptor, DamageModifiersUtils.DamageSource.NotApplicable)
    {
        ID = ("inc" + Name).GetHashCode();
        Incoming = true;
    }

    public override int GetTotalDamage(AbstractSingleActor actor, ParsedEvtcLog log, AbstractSingleActor? t, long start, long end)
    {
        FinalDefenses defenseData = actor.GetDefenseStats(t, log, start, end);
        return (CompareType) switch
        {
            DamageType.All                            => defenseData.DamageTaken,
            DamageType.Condition                      => defenseData.ConditionDamageTaken,
            DamageType.Power                          => defenseData.PowerDamageTaken,
            DamageType.LifeLeech                      => defenseData.LifeLeechDamageTaken,
            DamageType.Strike                         => defenseData.StrikeDamageTaken,
            DamageType.StrikeAndCondition             => defenseData.StrikeDamageTaken + defenseData.ConditionDamageTaken,
            DamageType.StrikeAndConditionAndLifeLeech => defenseData.StrikeDamageTaken + defenseData.ConditionDamageTaken + defenseData.LifeLeechDamageTaken,
            _ => throw new NotImplementedException("Not implemented damage type " + CompareType),
        };
    }

    public override IEnumerable<AbstractHealthDamageEvent> GetHitDamageEvents(AbstractSingleActor actor, ParsedEvtcLog log, AbstractSingleActor? t, long start, long end)
    {
        return actor.GetHitDamageTakenEvents(t, log, start, end, SrcType);
    }
    internal override AgentItem GetFoe(AbstractHealthDamageEvent evt)
    {
        return evt.From;
    }
}
