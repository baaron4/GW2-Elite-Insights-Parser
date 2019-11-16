using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{
    public class FinalDefenses
    {
        //public long allHealReceived;
        public long DamageTaken { get; set; }
        public int BlockedCount { get; set; }
        public int EvadedCount { get; set; }
        public int DodgeCount { get; set; }
        public int InvulnedCount { get; set; }
        public int DamageInvulned { get; set; }
        public int DamageBarrier { get; set; }
        public int InterruptedCount { get; set; }

        public FinalDefenses(ParsedLog log, PhaseData phase, AbstractSingleActor actor, AbstractSingleActor from)
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
