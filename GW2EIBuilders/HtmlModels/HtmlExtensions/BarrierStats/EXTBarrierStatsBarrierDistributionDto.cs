using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.EXTBarrier;


using BarrierDistributionItem = object[];

internal class EXTBarrierStatsBarrierDistributionDto
{

    public long ContributedBarrier { get; set; }
    public long TotalBarrier { get; set; }
    public long TotalCasting { get; set; }
    public List<BarrierDistributionItem>? Distribution { get; set; }

    private static BarrierDistributionItem GetBarrierToItem(SkillItem skill, IEnumerable<EXTBarrierEvent> barrierLogs, Dictionary<SkillItem, IEnumerable<CastEvent>>? castLogsBySkill, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBoons, BuffsContainer boons, PhaseData phase)
    {
        int totalbarrier = 0,
            minbarrier = int.MaxValue,
            maxbarrier = int.MinValue,
            hits = 0;
        bool isIndirectBarrier = false;
        foreach (EXTBarrierEvent dl in barrierLogs)
        {
            isIndirectBarrier = isIndirectBarrier || dl is EXTNonDirectBarrierEvent;
            int curdmg = dl.BarrierGiven;
            totalbarrier += curdmg;
            hits++;
            if (curdmg < minbarrier) { minbarrier = curdmg; }
            if (curdmg > maxbarrier) { maxbarrier = curdmg; }
        }

        if (isIndirectBarrier)
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

        IEnumerable<CastEvent>? clList = null;
        if (castLogsBySkill != null && castLogsBySkill.Remove(skill, out clList))
        {
            isIndirectBarrier = false;
        }

        long timeSpentCasting = 0, timeSpentCastingNoInterrupt = 0;
        int numberOfCast = 0, numberOfCastNoInterrupt = 0, timeWasted = 0, timeSaved = 0;
        long minTimeSpentCasting = 0, maxTimeSpentCasting = 0;
        if (clList != null)
        {
            (timeSpentCasting, timeSpentCastingNoInterrupt, minTimeSpentCasting, maxTimeSpentCasting, numberOfCast, numberOfCastNoInterrupt, timeSaved, timeWasted) = DamageDistributionDto.GetCastValues(clList, phase);
        }

        return [
                isIndirectBarrier,
                skill.ID,
                totalbarrier,
                minbarrier == int.MaxValue ? 0 : minbarrier,
                maxbarrier == int.MinValue ? 0 : maxbarrier,
                isIndirectBarrier ? 0 : numberOfCast,
                isIndirectBarrier ? 0 : -timeWasted / 1000.0,
                isIndirectBarrier ? 0 : timeSaved / 1000.0,
                hits,
                isIndirectBarrier ? 0 : timeSpentCasting,
                isIndirectBarrier ? 0 : minTimeSpentCasting,
                isIndirectBarrier ? 0 : maxTimeSpentCasting,
                isIndirectBarrier ? 0 : timeSpentCastingNoInterrupt,
                isIndirectBarrier ? 0 : numberOfCastNoInterrupt
        ];
    }

    public static EXTBarrierStatsBarrierDistributionDto BuildIncomingBarrierDistData(ParsedEvtcLog log, SingleActor p, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var barrierLogs = p.EXTBarrier.GetIncomingBarrierEvents(null, log, phase.Start, phase.End);
        var distribution = barrierLogs.GroupBy(x => x.Skill)
            .Select(group => GetBarrierToItem(group.Key, group, null, usedSkills, usedBuffs, log.Buffs, phase))
            .ToList();
        EXTFinalIncomingBarrierStat incomingBarrierStats = p.EXTBarrier.GetIncomingBarrierStats(null, log, phase.Start, phase.End);
        return new EXTBarrierStatsBarrierDistributionDto
        {
            Distribution = distribution,
            ContributedBarrier = incomingBarrierStats.BarrierReceived
        };
    }


    private static List<BarrierDistributionItem> BuildBarrierDistBodyData(ParsedEvtcLog log, IEnumerable<CastEvent> casting, IEnumerable<EXTBarrierEvent> barrierLogs, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, PhaseData phase)
    {
        var castLogsBySkill = casting.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.AsEnumerable());
        var list = barrierLogs.GroupBy(x => x.Skill)
            .Select(group => GetBarrierToItem(group.Key, group, castLogsBySkill, usedSkills, usedBuffs, log.Buffs, phase))
            .ToList();
        return list;
    }

    private static EXTBarrierStatsBarrierDistributionDto BuildBarrierDistDataInternal(ParsedEvtcLog log, EXTFinalOutgoingBarrierStat outgoingBarrierStats, SingleActor p, SingleActor? target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var casting = p.GetIntersectingCastEvents(log, phase.Start, phase.End);
        var barrierLogs = p.EXTBarrier.GetJustActorOutgoingBarrierEvents(target, log, phase.Start, phase.End);
        var dto = new EXTBarrierStatsBarrierDistributionDto
        {
            ContributedBarrier = outgoingBarrierStats.ActorBarrier,
            TotalBarrier = outgoingBarrierStats.Barrier,
            TotalCasting = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start)),
            Distribution = BuildBarrierDistBodyData(log, casting, barrierLogs, usedSkills, usedBuffs, phase)
        };
        return dto;
    }


    public static EXTBarrierStatsBarrierDistributionDto BuildFriendlyBarrierDistData(ParsedEvtcLog log, SingleActor actor, SingleActor? target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        EXTFinalOutgoingBarrierStat outgoingBarrierStats = actor.EXTBarrier.GetOutgoingBarrierStats(target, log, phase.Start, phase.End);
        return BuildBarrierDistDataInternal(log, outgoingBarrierStats, actor, target, phase, usedSkills, usedBuffs);
    }

    private static EXTBarrierStatsBarrierDistributionDto BuildBarrierDistDataMinionsInternal(ParsedEvtcLog log, EXTFinalOutgoingBarrierStat outgoingBarrierStats, Minions minions, SingleActor? target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var dto = new EXTBarrierStatsBarrierDistributionDto();
        var casting = minions.GetIntersectingCastEvents(log, phase.Start, phase.End);
        var barrierLogs = minions.EXTBarrier.GetOutgoingBarrierEvents(target, log, phase.Start, phase.End);
        dto.ContributedBarrier = barrierLogs.Sum(x => x.BarrierGiven);
        dto.TotalBarrier = outgoingBarrierStats.Barrier;
        //TODO(Rennorb) @perf
        dto.TotalCasting = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start));
        dto.Distribution = BuildBarrierDistBodyData(log, casting, barrierLogs, usedSkills, usedBuffs, phase);
        return dto;
    }

    public static EXTBarrierStatsBarrierDistributionDto BuildFriendlyMinionBarrierDistData(ParsedEvtcLog log, SingleActor actor, Minions minions, SingleActor? target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        EXTFinalOutgoingBarrierStat outgoingBarrierStats = actor.EXTBarrier.GetOutgoingBarrierStats(target, log, phase.Start, phase.End);

        return BuildBarrierDistDataMinionsInternal(log, outgoingBarrierStats, minions, target, phase, usedSkills, usedBuffs);
    }

    public static EXTBarrierStatsBarrierDistributionDto BuildFriendlyMinionIncomingBarrierDistData(ParsedEvtcLog log, Minions minions, SingleActor? target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var dto = new EXTBarrierStatsBarrierDistributionDto();
        var barrierLogs = minions.EXTBarrier.GetIncomingBarrierEvents(target, log, phase.Start, phase.End);
        dto.ContributedBarrier = barrierLogs.Sum(x => x.BarrierGiven);
        dto.Distribution = barrierLogs.GroupBy(x => x.Skill)
            .Select(group => GetBarrierToItem(group.Key, group, null, usedSkills, usedBuffs, log.Buffs, phase))
            .ToList();
        return dto;
    }
}
