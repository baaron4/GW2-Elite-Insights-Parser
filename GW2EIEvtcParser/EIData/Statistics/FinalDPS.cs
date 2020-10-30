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
        // Actor only
        public int ActorDps { get; internal set; }
        public int ActorDamage { get; internal set; }
        public int ActorCondiDps { get; internal set; }
        public int ActorCondiDamage { get; internal set; }
        public int ActorPowerDps { get; internal set; }
        public int ActorPowerDamage { get; internal set; }


        internal FinalDPS(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor actor, AbstractSingleActor target)
        {
            double phaseDuration = (phase.DurationInMS) / 1000.0;
            int damage;
            double dps = 0.0;
            IReadOnlyList<AbstractDamageEvent> damageLogs = actor.GetDamageLogs(target, log, phase.Start, phase.End);
            //DPS
            damage = damageLogs.Sum(x => x.Damage);

            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            Dps = (int)Math.Round(dps);
            Damage = damage;
            //Condi DPS
            damage = damageLogs.Sum(x => x.IsCondi(log) ? x.Damage : 0);

            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            CondiDps = (int)Math.Round(dps);
            CondiDamage = damage;
            //Power DPS
            damage = Damage - CondiDamage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            PowerDps = (int)Math.Round(dps);
            PowerDamage = damage;
            IReadOnlyList<AbstractDamageEvent> actorDamageLogs = actor.GetJustActorDamageLogs(target, log, phase.Start, phase.End);
            // Actor DPS
            damage = actorDamageLogs.Sum(x => x.Damage);

            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            ActorDps = (int)Math.Round(dps);
            ActorDamage = damage;
            //Actor Condi DPS
            damage = actorDamageLogs.Sum(x => x.IsCondi(log) ? x.Damage : 0);

            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            ActorCondiDps = (int)Math.Round(dps);
            ActorCondiDamage = damage;
            //Actor Power DPS
            damage = ActorDamage - ActorCondiDamage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            ActorPowerDps = (int)Math.Round(dps);
            ActorPowerDamage = damage;
        }
    }
}
