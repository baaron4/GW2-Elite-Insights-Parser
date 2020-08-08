using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class FinalDefenses
    {
        //public long allHealReceived;
        public long DamageTaken { get; }
        public int BlockedCount { get; }
        public int EvadedCount { get; }
        public int DodgeCount { get; }
        public int InvulnedCount { get; }
        public int DamageInvulned { get; }
        public int DamageBarrier { get; }
        public int InterruptedCount { get; }

        internal FinalDefenses(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor actor, AbstractSingleActor from)
        {
            long start = phase.Start;
            long end = phase.End;
            List<AbstractDamageEvent> damageLogs = actor.GetDamageTakenLogs(from, log, start, end);

            DamageTaken = damageLogs.Sum(x => (long)x.Damage);
            BlockedCount = damageLogs.Count(x => x.IsBlocked);
            InvulnedCount = 0;
            DamageInvulned = 0;
            EvadedCount = damageLogs.Count(x => x.IsEvaded);
            DodgeCount = actor.GetCastLogs(log, start, end).Count(x => x.SkillId == SkillItem.DodgeId || x.SkillId == SkillItem.MirageCloakDodgeId);
            DamageBarrier = damageLogs.Sum(x => x.ShieldDamage);
            InterruptedCount = damageLogs.Count(x => x.HasInterrupted);
            foreach (AbstractDamageEvent dl in damageLogs.Where(x => x.IsAbsorbed))
            {
                InvulnedCount++;
                DamageInvulned += dl.Damage;
            }
        }
    }
}
