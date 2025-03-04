using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.EXTHealing;

using HealingDistributionItem = object[];

internal class EXTHealingStatsHealingDistributionDto
{
    public long ContributedHealing { get; set; }
    public long ContributedDownedHealing { get; set; }
    public long TotalHealing { get; set; }
    public long TotalCasting { get; set; }
    public List<HealingDistributionItem>? Distribution { get; set; }

    private static HealingDistributionItem GetHealingToItem(SkillItem skill, IEnumerable<EXTHealingEvent> healingLogs, Dictionary<SkillItem, IEnumerable<CastEvent>>? castLogsBySkill, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBoons, BuffsContainer boons, PhaseData phase)
    {
        int totalhealing = 0,
            totaldownedhealing = 0,
                minhealing = int.MaxValue,
                maxhealing = int.MinValue,
                hits = 0;
        bool isIndirectHealing = false;
        foreach (EXTHealingEvent dl in healingLogs)
        {
            isIndirectHealing = isIndirectHealing || dl is EXTNonDirectHealingEvent;
            int curdmg = dl.HealingDone;
            totalhealing += curdmg;
            hits++;
            if (curdmg < minhealing) { minhealing = curdmg; }
            if (curdmg > maxhealing) { maxhealing = curdmg; }
            if (dl.AgainstDowned)
            {
                totaldownedhealing += dl.HealingDone;
            }

        }
        if (isIndirectHealing)
        {
            if (!usedBoons.ContainsKey(skill.ID))
            {
                if (boons.BuffsByIds.TryGetValue(skill.ID, out var buff))
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
            isIndirectHealing = false;
        }

        long timeSpentCasting = 0, timeSpentCastingNoInterrupt = 0;
        int numberOfCast = 0, numberOfCastNoInterrupt = 0, timeWasted = 0, timeSaved = 0;
        long minTimeSpentCasting = 0, maxTimeSpentCasting = 0;
        if (clList != null)
        {
            (timeSpentCasting, timeSpentCastingNoInterrupt, minTimeSpentCasting, maxTimeSpentCasting, numberOfCast, numberOfCastNoInterrupt, timeSaved, timeWasted) = DamageDistributionDto.GetCastValues(clList, phase);
        }
        return [
                isIndirectHealing,
                skill.ID,
                totalhealing,
                minhealing == int.MaxValue ? 0 : minhealing,
                maxhealing == int.MinValue ? 0 : maxhealing,
                isIndirectHealing ? 0 : numberOfCast,
                isIndirectHealing ? 0 : -timeWasted / 1000.0,
                isIndirectHealing ? 0 : timeSaved / 1000.0,
                hits,
                isIndirectHealing ? 0 : timeSpentCasting,
                totaldownedhealing,
                isIndirectHealing ? 0 : minTimeSpentCasting,
                isIndirectHealing ? 0 : maxTimeSpentCasting,
                isIndirectHealing ? 0 : timeSpentCastingNoInterrupt,
                isIndirectHealing ? 0 : numberOfCastNoInterrupt
           ];
    }

    public static EXTHealingStatsHealingDistributionDto BuildIncomingHealingDistData(ParsedEvtcLog log, SingleActor p, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        EXTFinalIncomingHealingStat incomingHealingStats = p.EXTHealing.GetIncomingHealStats(null, log, phase.Start, phase.End);
        var dto = new EXTHealingStatsHealingDistributionDto
        {
            Distribution = [],
            ContributedHealing = incomingHealingStats.Healed,
            ContributedDownedHealing = incomingHealingStats.DownedHealed
        };
        var healingLogs = p.EXTHealing.GetIncomingHealEvents(null, log, phase.Start, phase.End);
        foreach (var group in healingLogs.GroupBy(x => x.Skill))
        {
            dto.Distribution.Add(GetHealingToItem(group.Key, group, null, usedSkills, usedBuffs, log.Buffs, phase));
        }
        return dto;
    }


    private static List<HealingDistributionItem> BuildHealingDistBodyData(ParsedEvtcLog log, IEnumerable<CastEvent> casting, IEnumerable<EXTHealingEvent> healingLogs, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, PhaseData phase)
    {
        var list = new List<HealingDistributionItem>();
        var castLogsBySkill = casting.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.AsEnumerable());
        foreach (var group in healingLogs.GroupBy(x => x.Skill))
        {
            list.Add(GetHealingToItem(group.Key, group, castLogsBySkill, usedSkills, usedBuffs, log.Buffs, phase));
        }
        return list;
    }

    private static EXTHealingStatsHealingDistributionDto BuildHealingDistDataInternal(ParsedEvtcLog log, EXTFinalOutgoingHealingStat outgoingHealingStats, SingleActor p, SingleActor? target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var dto = new EXTHealingStatsHealingDistributionDto();
        var casting = p.GetIntersectingCastEvents(log, phase.Start, phase.End);
        var healingLogs = p.EXTHealing.GetJustActorOutgoingHealEvents(target, log, phase.Start, phase.End);
        dto.ContributedHealing = outgoingHealingStats.ActorHealing;
        dto.ContributedDownedHealing = outgoingHealingStats.ActorDownedHealing;
        dto.TotalHealing = outgoingHealingStats.Healing;
        dto.TotalCasting = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start));
        dto.Distribution = BuildHealingDistBodyData(log, casting, healingLogs, usedSkills, usedBuffs, phase);
        return dto;
    }


    public static EXTHealingStatsHealingDistributionDto BuildFriendlyHealingDistData(ParsedEvtcLog log, SingleActor actor, SingleActor? target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        EXTFinalOutgoingHealingStat outgoingHealingStats = actor.EXTHealing.GetOutgoingHealStats(target, log, phase.Start, phase.End);
        return BuildHealingDistDataInternal(log, outgoingHealingStats, actor, target, phase, usedSkills, usedBuffs);
    }

    private static EXTHealingStatsHealingDistributionDto BuildHealingDistDataMinionsInternal(ParsedEvtcLog log, EXTFinalOutgoingHealingStat outgoingHealingStats, Minions minions, SingleActor? target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var dto = new EXTHealingStatsHealingDistributionDto();
        var casting = minions.GetIntersectingCastEvents(log, phase.Start, phase.End);
        var healingLogs = minions.EXTHealing.GetOutgoingHealEvents(target, log, phase.Start, phase.End);
        dto.ContributedHealing = healingLogs.Sum(x => x.HealingDone);
        dto.TotalHealing = outgoingHealingStats.Healing;
        dto.TotalCasting = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start));
        dto.Distribution = BuildHealingDistBodyData(log, casting, healingLogs, usedSkills, usedBuffs, phase);
        return dto;
    }

    public static EXTHealingStatsHealingDistributionDto BuildFriendlyMinionHealingDistData(ParsedEvtcLog log, SingleActor actor, Minions minions, SingleActor? target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        EXTFinalOutgoingHealingStat outgoingHealingStats = actor.EXTHealing.GetOutgoingHealStats(target, log, phase.Start, phase.End);

        return BuildHealingDistDataMinionsInternal(log, outgoingHealingStats, minions, target, phase, usedSkills, usedBuffs);
    }
}
