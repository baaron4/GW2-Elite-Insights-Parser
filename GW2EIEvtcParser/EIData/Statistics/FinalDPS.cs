using System;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class FinalDPS
    {
        // Total
        public int Dps { get; set; }
        public int Damage { get; set; }
        public int CondiDps { get; set; }
        public int CondiDamage { get; set; }
        public int PowerDps { get; set; }
        public int PowerDamage { get; set; }
        // Actor only
        public int ActorDps { get; set; }
        public int ActorDamage { get; set; }
        public int ActorCondiDps { get; set; }
        public int ActorCondiDamage { get; set; }
        public int ActorPowerDps { get; set; }
        public int ActorPowerDamage { get; set; }


        public FinalDPS(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor actor, AbstractSingleActor target)
        {
            double phaseDuration = (phase.DurationInMS) / 1000.0;
            int damage;
            double dps = 0.0;
            //DPS
            damage = actor.GetDamageLogs(target, log, phase).Sum(x => x.Damage);

            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            Dps = (int)Math.Round(dps);
            Damage = damage;
            //Condi DPS
            damage = actor.GetDamageLogs(target, log, phase).Sum(x => x.IsCondi(log) ? x.Damage : 0);

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
            // Actor DPS
            damage = actor.GetJustPlayerDamageLogs(target, log, phase).Sum(x => x.Damage);

            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            ActorDps = (int)Math.Round(dps);
            ActorDamage = damage;
            //Actor Condi DPS
            damage = actor.GetJustPlayerDamageLogs(target, log, phase).Sum(x => x.IsCondi(log) ? x.Damage : 0);

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
