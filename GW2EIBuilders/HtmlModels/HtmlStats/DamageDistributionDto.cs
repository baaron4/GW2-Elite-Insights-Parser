using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.HTMLStats;

using DistributionItem = object[];
internal class DamageDistributionDto
{
    public static readonly DamageDistributionDto EmptyInstance = new();

    public long            ContributedDamage;
    public double          ContributedBreakbarDamage;
    public long            ContributedShieldDamage;
    public long            TotalDamage;
    public double          TotalBreakbarDamage;
    public long            TotalCasting;
    public List<DistributionItem>? Distribution;

    internal static (long timeSpentCasting, long timeSpentCastingNoInterrupt, long minTimeSpentCastingNoInterrupt, long maxTimeSpentCastingNoInterrupt, int numberOfCast, int numberOfCastNoInterrupt, int timeSaved, int timeWasted) GetCastValues(IEnumerable<CastEvent> clList, PhaseData phase)
    {
        long timeSpentCasting = 0;
        long timeSpentCastingNoInterrupt = 0;
        int numberOfCast = 0, numberOfCastNoInterrupt = 0, timeWasted = 0, timeSaved = 0;
        long minTimeSpentCastingNoInterrupt = long.MaxValue, maxTimeSpentCastingNoInterrupt = long.MinValue;
        foreach (CastEvent cl in clList)
        {
            if (phase.InInterval(cl.Time))
            {
                numberOfCast++;
                switch (cl.Status)
                {
                    case CastEvent.AnimationStatus.Interrupted:
                        timeWasted += cl.SavedDuration;
                        break;

                    case CastEvent.AnimationStatus.Reduced:
                        timeSaved += cl.SavedDuration;
                        break;
                }
                if (cl.IsReduced || cl.IsFull)
                {
                    timeSpentCastingNoInterrupt += cl.ActualDuration;
                    numberOfCastNoInterrupt++;
                    minTimeSpentCastingNoInterrupt = Math.Min(cl.ActualDuration, minTimeSpentCastingNoInterrupt);
                    maxTimeSpentCastingNoInterrupt = Math.Max(cl.ActualDuration, maxTimeSpentCastingNoInterrupt);
                }
            }
            long castTime = Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start);

            timeSpentCasting += castTime;
        }
        if (timeSpentCasting == 0)
        {
            minTimeSpentCastingNoInterrupt = 0;
            maxTimeSpentCastingNoInterrupt = 0;
        }
        return (timeSpentCasting, timeSpentCastingNoInterrupt, minTimeSpentCastingNoInterrupt, maxTimeSpentCastingNoInterrupt, numberOfCast, numberOfCastNoInterrupt, timeSaved, timeWasted);
    }

    private static DistributionItem GetDamageEventtoItem(SkillItem skill, IEnumerable<HealthDamageEvent> damageLogs, Dictionary<SkillItem, IEnumerable<CastEvent>>? castLogsBySkill, Dictionary<SkillItem, IEnumerable<BreakbarDamageEvent>> breakbarLogsBySkill, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBoons, BuffsContainer boons, PhaseData phase)
    {
        int totaldamage = 0,
            mindamage = int.MaxValue,
            maxdamage = int.MinValue,
            hits = 0,
            crit = 0,
            critDamage = 0,
            connectedHits = 0,
            flank = 0,
            againstMoving = 0,
            glance = 0,
            shieldDamage = 0;
        bool IsIndirectDamage = false;
        foreach (HealthDamageEvent dl in damageLogs)
        {
            IsIndirectDamage = IsIndirectDamage || dl is NonDirectHealthDamageEvent;
            int curdmg = dl.HealthDamage;
            totaldamage += curdmg;
            hits += dl.DoubleProcHit ? 0 : 1;
            if (dl.HasHit)
            {
                if (curdmg < mindamage) { mindamage = curdmg; }
                if (curdmg > maxdamage) { maxdamage = curdmg; }
                connectedHits++;
                if (dl.HasCrit)
                {
                    crit++;
                    critDamage += dl.HealthDamage;
                }
                if (dl.HasGlanced)
                {
                    glance++;
                }

                if (dl.IsFlanking)
                {
                    flank++;
                }
                if (dl.AgainstMoving)
                {
                    againstMoving++;
                }
            }

            shieldDamage += dl.ShieldDamage;
        }

        if (IsIndirectDamage)
        {
            if (!usedBoons.ContainsKey(skill.ID))
            {
                if (boons.BuffsByIDs.TryGetValue(skill.ID, out var buff))
                {
                    usedBoons.Add(buff.ID, buff);
                }
                else
                {
                    SkillItem aux = skill;
                    var auxBoon = new Buff(aux.Name, aux.ID, aux.Icon);
                    usedBoons.Add(auxBoon.ID, auxBoon);
                }
            }
        }
        else
        {
            usedSkills.TryAdd(skill.ID, skill);
        }

        long timeSpentCasting = 0;
        long timeSpentCastingNoInterrupt = 0;
        int numberOfCast = 0, numberOfCastNoInterrupt = 0, timeWasted = 0, timeSaved = 0;
        long minTimeSpentCastingNoInterrupt = 0, maxTimeSpentCastingNoInterrupt = 0;
        if (!IsIndirectDamage && castLogsBySkill != null && castLogsBySkill.Remove(skill, out var clList))
        {
            (timeSpentCasting, timeSpentCastingNoInterrupt, minTimeSpentCastingNoInterrupt, maxTimeSpentCastingNoInterrupt, numberOfCast, numberOfCastNoInterrupt, timeSaved, timeWasted) = GetCastValues(clList, phase);
        }

        double breakbarDamage = breakbarLogsBySkill.Remove(skill, out var brList) ? Math.Round(brList.Sum(x => x.BreakbarDamage), 1) : 0;

        return [
            IsIndirectDamage,
            skill.ID,
            totaldamage,
            mindamage == int.MaxValue ? 0 : mindamage,
            maxdamage == int.MinValue ? 0 : maxdamage,
            IsIndirectDamage ? 0 : numberOfCast,
            connectedHits,
            IsIndirectDamage ? 0 : crit,
            IsIndirectDamage ? 0 : flank,
            IsIndirectDamage ? 0 : glance,
            IsIndirectDamage ? 0 : -timeWasted / 1000.0,
            IsIndirectDamage ? 0 : timeSaved / 1000.0,
            shieldDamage,
            IsIndirectDamage ? 0 : critDamage,
            hits,
            IsIndirectDamage ? 0 : timeSpentCasting,
            againstMoving,
            Math.Round(breakbarDamage, 1),
            IsIndirectDamage ? 0 : minTimeSpentCastingNoInterrupt,
            IsIndirectDamage ? 0 : maxTimeSpentCastingNoInterrupt,
            IsIndirectDamage ? 0 : timeSpentCastingNoInterrupt,
            IsIndirectDamage ? 0 : numberOfCastNoInterrupt
        ];
    }


    private static List<DistributionItem> BuildDamageDistributionTakenBodyData(ParsedEvtcLog log, IEnumerable<HealthDamageEvent> damageLogs, IEnumerable<BreakbarDamageEvent> breakbarLogs, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, PhaseData phase)
    {
        var list = new List<DistributionItem>();
        //NOTE(Rennorb): The inner list only ever gets enumerated once, because once its matched it gets removed.
        var breakbarLogsBySkill = breakbarLogs.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.AsEnumerable());
        foreach (var group in damageLogs.GroupBy(x => x.Skill))
        {
            list.Add(GetDamageEventtoItem(group.Key, group.ToList(), [], breakbarLogsBySkill, usedSkills, usedBuffs, log.Buffs, phase));
        }
        // breakbar only
        foreach (var (skill, events) in breakbarLogsBySkill)
        {
            usedSkills.TryAdd(skill.ID, skill);
            double breakbarDamage = Math.Round(events.Sum(x => x.BreakbarDamage), 1);
            var hits = events.Count();
            list.Add([
                false,
                skill.ID,
                0,
                0,
                0,
                0,
                hits,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                hits,
                0,
                0,
                breakbarDamage,
                0,
                0,
                0,
                0
           ]);
        }
        return list;
    }

    private static List<DistributionItem> BuildDamageDistributionBodyData(ParsedEvtcLog log, IEnumerable<CastEvent> casting, IEnumerable<HealthDamageEvent> damageLogs, IEnumerable<BreakbarDamageEvent> breakbarLogs, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, PhaseData phase)
    {
        var list = new List<DistributionItem>();
        var castLogsBySkill = casting.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.AsEnumerable());
        //NOTE(Rennorb): The inner list only ever gets enumerated once, because once its matched it gets removed.
        var breakbarLogsBySkill = breakbarLogs.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.AsEnumerable());
        foreach (var group in damageLogs.GroupBy(x => x.Skill))
        {
            list.Add(GetDamageEventtoItem(group.Key, group.ToList(), castLogsBySkill, breakbarLogsBySkill, usedSkills, usedBuffs, log.Buffs, phase));
        }
        // non damaging
        foreach (var pair in castLogsBySkill)
        {
            if (!usedSkills.ContainsKey(pair.Key.ID))
            {
                usedSkills.Add(pair.Key.ID, pair.Key);
            }
            double breakbarDamage = breakbarLogsBySkill.Remove(pair.Key, out var brList) ? Math.Round(brList.Sum(x => x.BreakbarDamage), 1) : 0;

            var hits = brList != null ? brList.Count() : 0;
            (long timeSpentCasting, long timeSpentCastingNoInterrupt, long minTimeSpentCastingNoInterrupt, long maxTimeSpentCastingNoInterrupt, int numberOfCast, int numberOfCastNoInterrupt, int timeSaved, int timeWasted) = GetCastValues(pair.Value, phase);


            list.Add([
                false,
                pair.Key.ID,
                0,
                0,
                0,
                numberOfCast,
                hits,
                0,
                0,
                0,
                -timeWasted / 1000.0,
                timeSaved / 1000.0,
                0,
                0,
                hits,
                timeSpentCasting,
                0,
                Math.Round(breakbarDamage, 1),
                minTimeSpentCastingNoInterrupt,
                maxTimeSpentCastingNoInterrupt,
                timeSpentCastingNoInterrupt,
                numberOfCastNoInterrupt
                ]);
        }
        // breakbar only
        foreach (var (skill, events) in breakbarLogsBySkill)
        {
            usedSkills.TryAdd(skill.ID, skill);
            double breakbarDamage = Math.Round(events.Sum(x => x.BreakbarDamage), 1);
            var hits = events.Count();

            list.Add([
                false,
                skill.ID,
                0,
                0,
                0,
                hits,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                hits,
                0,
                0,
                0,
                breakbarDamage,
                0,
                0,
                0,
                0
           ]);
        }
        return list;
    }


    public static DamageDistributionDto BuildDamageDistributionData(ParsedEvtcLog log, SingleActor actor, SingleActor? target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        DamageStatistics dps = actor.GetDamageStats(target, log, phase.Start, phase.End);
        var casting = actor.GetIntersectingCastEvents(log, phase.Start, phase.End);
        var damageLogs = actor.GetJustActorDamageEvents(target, log, phase.Start, phase.End);
        var breakbarLogs = actor.GetJustActorBreakbarDamageEvents(target, log, phase.Start, phase.End);
        var dto = new DamageDistributionDto
        {
            ContributedDamage = dps.ActorDamage,
            ContributedShieldDamage = dps.ActorBarrierDamage,
            ContributedBreakbarDamage = dps.ActorBreakbarDamage,
            TotalDamage = dps.Damage,
            TotalBreakbarDamage = dps.BreakbarDamage,
            TotalCasting = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start)),
            Distribution = BuildDamageDistributionBodyData(log, casting, damageLogs, breakbarLogs, usedSkills, usedBuffs, phase)
        };
        return dto;
    }

    public static DamageDistributionDto BuildDamageTakenDistributionData(ParsedEvtcLog log, SingleActor actor, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        DefenseAllStatistics? incomingDamageStats = actor.GetDefenseStats(log, phase.Start, phase.End);
        var damageLogs = actor.GetDamageTakenEvents(null, log, phase.Start, phase.End);
        var breakbarLogs = actor.GetBreakbarDamageTakenEvents(null, log, phase.Start, phase.End);
        var breakbarLogsBySkill = breakbarLogs.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.AsEnumerable());
        var dto = new DamageDistributionDto
        {
            Distribution = BuildDamageDistributionTakenBodyData(log, damageLogs, breakbarLogs, usedSkills, usedBuffs, phase),
            ContributedDamage = incomingDamageStats.DamageTaken,
            ContributedShieldDamage = incomingDamageStats.DamageBarrier,
            ContributedBreakbarDamage = incomingDamageStats.BreakbarDamageTaken
        };
        return dto;
    }

    public static DamageDistributionDto BuildMinionsDamageDistributionData(ParsedEvtcLog log, Minions minions, SingleActor? target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        DamageStatistics dps = minions.Master.GetDamageStats(target, log, phase.Start, phase.End);
        var casting = minions.GetIntersectingCastEvents(log, phase.Start, phase.End);
        var damageLogs = minions.GetDamageEvents(target, log, phase.Start, phase.End);
        var brkDamageLogs = minions.GetBreakbarDamageEvents(target, log, phase.Start, phase.End);
        var dto = new DamageDistributionDto
        {
            ContributedDamage = damageLogs.Sum(x => x.HealthDamage),
            ContributedShieldDamage = damageLogs.Sum(x => x.ShieldDamage),
            ContributedBreakbarDamage = Math.Round(brkDamageLogs.Sum(x => x.BreakbarDamage), 1),
            TotalDamage = dps.Damage,
            TotalBreakbarDamage = dps.BreakbarDamage,
            TotalCasting = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start)),
            Distribution = BuildDamageDistributionBodyData(log, casting, damageLogs, brkDamageLogs, usedSkills, usedBuffs, phase)
        };
        return dto;
    }

    public static DamageDistributionDto BuildMinionsDamageTakenDistributionData(ParsedEvtcLog log, Minions minions, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var damageLogs = minions.GetDamageTakenEvents(null, log, phase.Start, phase.End);
        var breakbarLogs = minions.GetBreakbarDamageTakenEvents(null, log, phase.Start, phase.End);
        var dto = new DamageDistributionDto
        {
            ContributedDamage = damageLogs.Sum(x => x.HealthDamage),
            ContributedShieldDamage = damageLogs.Sum(x => x.ShieldDamage),
            ContributedBreakbarDamage = Math.Round(breakbarLogs.Sum(x => x.BreakbarDamage), 1),
            Distribution = BuildDamageDistributionTakenBodyData(log, damageLogs, breakbarLogs, usedSkills, usedBuffs, phase)
        };
        return dto;
    }
}
