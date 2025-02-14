using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class FinalDPS
{
    // Total
    public readonly int DPS;
    public readonly int Damage;
    public readonly int ConditionDPS;
    public readonly int ConditionDamage;
    public readonly int PowerDPS;
    public readonly int PowerDamage;
    public readonly int StrikeDPS;
    public readonly int StrikeDamage;
    public readonly int LifeLeechDPS;
    public readonly int LifeLeechDamage;
    public readonly double BreakbarDamage;
    public readonly int BarrierDPS;
    public readonly int BarrierDamage;
    // Actor only
    public readonly int ActorDPS;
    public readonly int ActorDamage;
    public readonly int ActorConditionDPS;
    public readonly int ActorConditionDamage;
    public readonly int ActorPowerDPS;
    public readonly int ActorPowerDamage;
    public readonly int ActorStrikeDPS;
    public readonly int ActorStrikeDamage;
    public readonly int ActorLifeLeechDPS;
    public readonly int ActorLifeLeechDamage;
    public readonly double ActorBreakbarDamage;
    public readonly int ActorBarrierDPS;
    public readonly int ActorBarrierDamage;


    internal FinalDPS(ParsedEvtcLog log, long start, long end, SingleActor actor, SingleActor? target)
    {
        double phaseDuration = (end - start) / 1000.0;
        (Damage, PowerDamage, ConditionDamage, StrikeDamage, LifeLeechDamage, BarrierDamage) = ComputeDamageFrom(log, actor.GetDamageEvents(target, log, start, end));
        (ActorDamage, ActorPowerDamage, ActorConditionDamage, ActorStrikeDamage, ActorLifeLeechDamage, ActorBarrierDamage) = ComputeDamageFrom(log, actor.GetJustActorDamageEvents(target, log, start, end));

        if (phaseDuration > 0)
        {
            DPS = (int)Math.Round(Damage / phaseDuration);
            ConditionDPS = (int)Math.Round(ConditionDamage / phaseDuration);
            PowerDPS = (int)Math.Round(PowerDamage / phaseDuration);
            StrikeDPS = (int)Math.Round(StrikeDamage / phaseDuration);
            LifeLeechDPS = (int)Math.Round(LifeLeechDamage / phaseDuration);
            BarrierDPS = (int)Math.Round(BarrierDamage / phaseDuration);
            //
            ActorDPS = (int)Math.Round(ActorDamage / phaseDuration);
            ActorConditionDPS = (int)Math.Round(ActorConditionDamage / phaseDuration);
            ActorPowerDPS = (int)Math.Round(ActorPowerDamage / phaseDuration);
            ActorStrikeDPS = (int)Math.Round(ActorStrikeDamage / phaseDuration);
            ActorLifeLeechDPS = (int)Math.Round(ActorLifeLeechDamage / phaseDuration);
            ActorBarrierDPS = (int)Math.Round(ActorBarrierDamage / phaseDuration);
        }

        // Breakbar 
        BreakbarDamage = Math.Round(actor.GetBreakbarDamageEvents(target, log, start, end).Sum(x => x.BreakbarDamage), 1);
        ActorBreakbarDamage = Math.Round(actor.GetJustActorBreakbarDamageEvents(target, log, start, end).Sum(x => x.BreakbarDamage), 1);
    }

    private static (int allDamage, int powerDamage, int conditionDamage, int strikeDamage, int lifeLeechDamage, int barrierDamage) ComputeDamageFrom(ParsedEvtcLog log, IEnumerable<HealthDamageEvent> damageEvents)
    {
        int allDamage = 0;
        int powerDamage = 0;
        int conditionDamage = 0;
        int strikeDamage = 0;
        int lifeLeechDamage = 0;
        int barrierDamage = 0;
        foreach (HealthDamageEvent damageEvent in damageEvents)
        {
            allDamage += damageEvent.HealthDamage;
            if (damageEvent is NonDirectHealthDamageEvent ndhd)
            {
                if (damageEvent.ConditionDamageBased(log))
                {
                    conditionDamage += damageEvent.HealthDamage;
                }
                else
                {
                    powerDamage += damageEvent.HealthDamage;
                    if (ndhd.IsLifeLeech)
                    {
                        lifeLeechDamage += damageEvent.HealthDamage;
                    }
                }
            }
            else
            {
                strikeDamage += damageEvent.HealthDamage;
                powerDamage += damageEvent.HealthDamage;
            }
            barrierDamage += damageEvent.ShieldDamage;
        }
        return (allDamage, powerDamage, conditionDamage, strikeDamage, lifeLeechDamage, barrierDamage);
    }
}
