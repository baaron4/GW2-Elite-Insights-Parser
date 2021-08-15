using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class FinalDPS
    {
        // Total
        public int Dps { get; internal set; }
        public int Damage { get; internal set; }
        public int CondiDps { get; internal set; }
        public int CondiDamage { get; internal set; }
        public int PowerDps { get; internal set; }
        public int PowerDamage { get; internal set; }
        public int StrikeDps { get; internal set; }
        public int StrikeDamage { get; internal set; }
        public int LifeLeechDps { get; internal set; }
        public int LifeLeechDamage { get; internal set; }
        public double BreakbarDamage { get; internal set; }
        public int BarrierDps { get; internal set; }
        public int BarrierDamage { get; internal set; }
        // Actor only
        public int ActorDps { get; internal set; }
        public int ActorDamage { get; internal set; }
        public int ActorCondiDps { get; internal set; }
        public int ActorCondiDamage { get; internal set; }
        public int ActorPowerDps { get; internal set; }
        public int ActorPowerDamage { get; internal set; }
        public int ActorStrikeDps { get; internal set; }
        public int ActorStrikeDamage { get; internal set; }
        public int ActorLifeLeechDps { get; internal set; }
        public int ActorLifeLeechDamage { get; internal set; }
        public double ActorBreakbarDamage { get; internal set; }
        public int ActorBarrierDps { get; internal set; }
        public int ActorBarrierDamage { get; internal set; }


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
                ActorDps = (int)Math.Round(Damage / phaseDuration);
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
