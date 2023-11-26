using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    public class FinalDefenses
    {
        //public long allHealReceived;
        public int DamageTaken { get; }
        public int ConditionDamageTaken { get; }
        public int PowerDamageTaken { get; }
        public int LifeLeechDamageTaken { get; }
        public int StrikeDamageTaken { get; }
        public int DownedDamageTaken { get; } 
        public double BreakbarDamageTaken { get; }
        public int BlockedCount { get; }
        public int MissedCount { get; }
        public int EvadedCount { get; }
        public int DodgeCount { get; }
        public int InvulnedCount { get; }
        public int DamageBarrier { get; }
        public int InterruptedCount { get; }
        public int BoonStrips { get; }
        public double BoonStripsTime { get; }
        public int ConditionCleanses { get; }
        public double ConditionCleansesTime { get; }

        private static (int, double) GetStripData(IReadOnlyList<Buff> buffs, ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor from, bool excludeSelf)
        {
            double stripTime = 0;
            int strip = 0;
            foreach (Buff buff in buffs)
            {
                double currentBoonStripTime = 0;
                IReadOnlyList<BuffRemoveAllEvent> removeAllArray = log.CombatData.GetBuffRemoveAllData(buff.ID);
                foreach (BuffRemoveAllEvent brae in removeAllArray)
                {
                    if (brae.Time >= start && brae.Time <= end && brae.To == actor.AgentItem)
                    {
                        if (from != null && brae.CreditedBy != from.AgentItem || brae.CreditedBy == ParserHelper._unknownAgent || (excludeSelf && brae.CreditedBy == actor.AgentItem))
                        {
                            continue;
                        }
                        currentBoonStripTime = Math.Max(currentBoonStripTime + brae.RemovedDuration, log.FightData.FightDuration);
                        strip++;
                    }
                }
                stripTime += currentBoonStripTime;
            }
            stripTime = Math.Round(stripTime / 1000.0, ParserHelper.TimeDigit);
            return (strip, stripTime);
        }

        internal FinalDefenses(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor from)
        {
            IReadOnlyList<AbstractHealthDamageEvent> damageLogs = actor.GetDamageTakenEvents(from, log, start, end);
            foreach (AbstractHealthDamageEvent damageEvent in damageLogs)
            {
                DamageTaken += damageEvent.HealthDamage;
                if (damageEvent is NonDirectHealthDamageEvent ndhd)
                {
                    if (damageEvent.ConditionDamageBased(log))
                    {
                        ConditionDamageTaken += damageEvent.HealthDamage;
                    }
                    else
                    {
                        PowerDamageTaken += damageEvent.HealthDamage;
                        if (ndhd.IsLifeLeech)
                        {
                            LifeLeechDamageTaken += damageEvent.HealthDamage;
                        }
                    }
                }
                else
                {
                    StrikeDamageTaken += damageEvent.HealthDamage;
                    PowerDamageTaken += damageEvent.HealthDamage;
                }
                DamageBarrier += damageEvent.ShieldDamage;
                if (damageEvent.IsBlocked)
                {
                    BlockedCount++;
                }
                if (damageEvent.IsBlind)
                {
                    MissedCount++;
                }
                if (damageEvent.IsAbsorbed)
                {
                    InvulnedCount++;
                }
                if (damageEvent.IsEvaded)
                {
                    EvadedCount++;
                }
                if (damageEvent.HasInterrupted)
                {
                    InterruptedCount++;
                }
                if (damageEvent.AgainstDowned)
                {
                    DownedDamageTaken += damageEvent.HealthDamage;
                }
            }
            DodgeCount = actor.GetCastEvents(log, start, end).Count(x => x.Skill.IsDodge(log.SkillData));
            BreakbarDamageTaken = Math.Round(actor.GetBreakbarDamageTakenEvents(from, log, start, end).Sum(x => x.BreakbarDamage), 1);
            (BoonStrips, BoonStripsTime) = GetStripData(log.Buffs.BuffsByClassification[BuffClassification.Boon], log, start, end, actor, from, true);
            (ConditionCleanses, ConditionCleansesTime) = GetStripData(log.Buffs.BuffsByClassification[BuffClassification.Condition], log, start, end, actor, from, false);
        }
    }
}
