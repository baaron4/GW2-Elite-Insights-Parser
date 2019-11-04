using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.Models
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
        public int DownCount { get; set; }
        public int DownDuration { get; set; }
        public int DeadCount { get; set; }
        public int DeadDuration { get; set; }
        public int DcCount { get; set; }
        public int DcDuration { get; set; }

        public FinalDefenses(ParsedLog log, PhaseData phase, AbstractSingleActor actor)
        {
            var dead = new List<(long start, long end)>();
            var down = new List<(long start, long end)>();
            var dc = new List<(long start, long end)>();
            actor.AgentItem.GetAgentStatus(dead, down, dc, log);
            long start = phase.Start;
            long end = phase.End;
            List<AbstractDamageEvent> damageLogs = actor.GetDamageTakenLogs(null, log, start, end);

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

            //		
            DownCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DownId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);
            DeadCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DeathId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);
            DcCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DCId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);

            DownDuration = (int)down.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
            DeadDuration = (int)dead.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
            DcDuration = (int)dc.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
        }
    }
}
