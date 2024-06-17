using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class FinalDPS
    {
        // Total
        public int Dps { get; }
        public int Damage { get; }
        public int CondiDps { get; }
        public int CondiDamage { get; }
        public int PowerDps { get; }
        public int PowerDamage { get; }
        public int StrikeDps { get; }
        public int StrikeDamage { get; }
        public int LifeLeechDps { get; }
        public int LifeLeechDamage { get; }
        public double BreakbarDamage { get; }
        public int BarrierDps { get; }
        public int BarrierDamage { get; }
        // Actor only
        public int ActorDps { get; }
        public int ActorDamage { get; }
        public int ActorCondiDps { get; }
        public int ActorCondiDamage { get; }
        public int ActorPowerDps { get; }
        public int ActorPowerDamage { get; }
        public int ActorStrikeDps { get; }
        public int ActorStrikeDamage { get; }
        public int ActorLifeLeechDps { get; }
        public int ActorLifeLeechDamage { get; }
        public double ActorBreakbarDamage { get; }
        public int ActorBarrierDps { get; }
        public int ActorBarrierDamage { get; }


        internal FinalDPS(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor target)
        {
            double phaseDuration = (end - start) / 1000.0;
            (Damage, PowerDamage, CondiDamage, StrikeDamage, LifeLeechDamage, BarrierDamage) = ComputeDamageFrom(log, actor.GetDamageEvents(target, log, start, end));
            (ActorDamage, ActorPowerDamage, ActorCondiDamage, ActorStrikeDamage, ActorLifeLeechDamage, ActorBarrierDamage) = ComputeDamageFrom(log, actor.GetJustActorDamageEvents(target, log, start, end));

            if (phaseDuration > 0)
            {
                Dps = (int)Math.Round(Damage / phaseDuration);
                CondiDps = (int)Math.Round(CondiDamage / phaseDuration);
                PowerDps = (int)Math.Round(PowerDamage / phaseDuration);
                StrikeDps = (int)Math.Round(StrikeDamage / phaseDuration);
                LifeLeechDps = (int)Math.Round(LifeLeechDamage / phaseDuration);
                BarrierDps = (int)Math.Round(BarrierDamage / phaseDuration);
                //
                ActorDps = (int)Math.Round(ActorDamage / phaseDuration);
                ActorCondiDps = (int)Math.Round(ActorCondiDamage / phaseDuration);
                ActorPowerDps = (int)Math.Round(ActorPowerDamage / phaseDuration);
                ActorStrikeDps = (int)Math.Round(ActorStrikeDamage / phaseDuration);
                ActorLifeLeechDps = (int)Math.Round(ActorLifeLeechDamage / phaseDuration);
                ActorBarrierDps = (int)Math.Round(ActorBarrierDamage / phaseDuration);
            }

            // Breakbar 
            BreakbarDamage = Math.Round(actor.GetBreakbarDamageEvents(target, log, start, end).Sum(x => x.BreakbarDamage), 1);
            ActorBreakbarDamage = Math.Round(actor.GetJustActorBreakbarDamageEvents(target, log, start, end).Sum(x => x.BreakbarDamage), 1);
        }

        private static (int allDamage, int powerDamage, int conditionDamage, int strikeDamage, int lifeLeechDamage, int barrierDamage) ComputeDamageFrom(ParsedEvtcLog log, IReadOnlyList<AbstractHealthDamageEvent> damageEvents)
        {
            int allDamage = 0;
            int powerDamage = 0;
            int conditionDamage = 0;
            int strikeDamage = 0;
            int lifeLeechDamage = 0;
            int barrierDamage = 0;
            foreach (AbstractHealthDamageEvent damageEvent in damageEvents)
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
}
