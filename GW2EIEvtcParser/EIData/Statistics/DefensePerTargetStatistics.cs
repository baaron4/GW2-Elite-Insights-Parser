using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData;

public class DefensePerTargetStatistics
{
    //public long allHealReceived;
    public readonly int DamageTaken;
    public readonly int DamageTakenCount;

    public readonly int DamageBarrier;
    public readonly int DamageBarrierCount;

    public readonly int ConditionDamageTaken;
    public readonly int ConditionDamageTakenCount;

    public readonly int PowerDamageTaken;
    public readonly int PowerDamageTakenCount;

    public readonly int LifeLeechDamageTaken;
    public readonly int LifeLeechDamageTakenCount;

    public readonly int StrikeDamageTaken;
    public readonly int StrikeDamageTakenCount;

    public readonly int DownedDamageTaken;
    public readonly int DownedDamageTakenCount;

    public readonly double BreakbarDamageTaken;
    public readonly int BreakbarDamageTakenCount;

    public readonly int BlockedCount;
    public readonly int MissedCount;
    public readonly int EvadedCount;
    public readonly int DodgeCount;
    public readonly int InvulnedCount;
    public readonly int InterruptedCount;

    public readonly int BoonStrips;
    public readonly double BoonStripsTime;
    public readonly int ConditionCleanses;
    public readonly double ConditionCleansesTime;

    public readonly int ReceivedCrowdControl;
    public readonly double ReceivedCrowdControlDuration;

    private static (int, double) GetStripData(IReadOnlyList<Buff> buffs, ParsedEvtcLog log, long start, long end, SingleActor actor, SingleActor? from, bool excludeSelf)
    {
        double stripTime = 0;
        int strip = 0;
        foreach (Buff buff in buffs)
        {
            double currentBoonStripTime = 0;
            IReadOnlyList<BuffRemoveAllEvent> removeAllArray = log.CombatData.GetBuffRemoveAllDataByDst(buff.ID, actor.AgentItem);
            foreach (BuffRemoveAllEvent brae in removeAllArray)
            {
                if (brae.Time >= start && brae.Time <= end)
                {
                    if ((from != null && !from.AgentItem.Is(brae.CreditedBy)) || 
                        brae.CreditedBy.IsUnknown || 
                        (excludeSelf && brae.CreditedBy.Is(actor.AgentItem)))
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

    internal DefensePerTargetStatistics(ParsedEvtcLog log, long start, long end, SingleActor actor, SingleActor? from)
    {
        var damageLogs = actor.GetDamageTakenEvents(from, log, start, end);
        foreach (HealthDamageEvent damageEvent in damageLogs)
        {
            if (damageEvent.HasHit)
            {
                DamageTaken += damageEvent.HealthDamage;
                DamageTakenCount++;
                if (damageEvent.ConditionDamageBased(log))
                {
                    ConditionDamageTaken += damageEvent.HealthDamage;
                    ConditionDamageTakenCount++;
                }
                else
                {
                    if (damageEvent is NonDirectHealthDamageEvent ndhd)
                    {
                        if (ndhd.IsLifeLeech)
                        {
                            LifeLeechDamageTaken += damageEvent.HealthDamage;
                            LifeLeechDamageTaken++;
                        }
                    }
                    else
                    {
                        StrikeDamageTaken += damageEvent.HealthDamage;
                        StrikeDamageTakenCount++;
                    }
                    PowerDamageTaken += damageEvent.HealthDamage;
                    PowerDamageTakenCount++;
                }
                if (damageEvent.ShieldDamage > 0)
                {
                    DamageBarrier += damageEvent.ShieldDamage;
                    DamageBarrierCount++;
                }
                if (damageEvent.AgainstDowned)
                {
                    DownedDamageTaken += damageEvent.HealthDamage;
                    DownedDamageTakenCount++;
                }
            }
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
        }
        var ccs = actor.GetIncomingCrowdControlEvents(from, log, start, end);
        foreach (CrowdControlEvent cc in ccs)
        {
            ReceivedCrowdControl++;
            ReceivedCrowdControlDuration += cc.Duration;
        }
        DodgeCount = actor.GetCastEvents(log, start, end).Count(x => x.Skill.IsDodge(log.SkillData));
        foreach (BreakbarDamageEvent brk in actor.GetBreakbarDamageTakenEvents(from, log, start, end))
        {
            BreakbarDamageTaken += brk.BreakbarDamage;
            BreakbarDamageTakenCount++;
        }
        BreakbarDamageTaken = Math.Round(BreakbarDamageTaken, 1);
        (BoonStrips, BoonStripsTime) = GetStripData(log.Buffs.BuffsByClassification[BuffClassification.Boon], log, start, end, actor, from, true);
        (ConditionCleanses, ConditionCleansesTime) = GetStripData(log.Buffs.BuffsByClassification[BuffClassification.Condition], log, start, end, actor, from, false);
    }
}
