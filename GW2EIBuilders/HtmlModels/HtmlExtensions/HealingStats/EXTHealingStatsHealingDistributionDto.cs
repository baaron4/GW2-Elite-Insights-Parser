using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.Extensions;
using GW2EIBuilders.HtmlModels.HTMLStats;

namespace GW2EIBuilders.HtmlModels.EXTHealing
{
    internal class EXTHealingStatsHealingDistributionDto
    {
        public long ContributedHealing { get; set; }
        public long ContributedDownedHealing { get; set; }
        public long TotalHealing { get; set; }
        public long TotalCasting { get; set; }
        public List<object[]> Distribution { get; set; }

        private static object[] GetHealingToItem(SkillItem skill, List<EXTAbstractHealingEvent> healingLogs, Dictionary<SkillItem, List<AbstractCastEvent>> castLogsBySkill, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBoons, BuffsContainer boons, PhaseData phase)
        {
            int totalhealing = 0,
                totaldownedhealing = 0,
                    minhealing = int.MaxValue,
                    maxhealing = int.MinValue,
                    hits = 0;
            bool isIndirectHealing = false;
            foreach (EXTAbstractHealingEvent dl in healingLogs)
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
                    if (boons.BuffsByIds.TryGetValue(skill.ID, out Buff buff))
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
                if (!usedSkills.ContainsKey(skill.ID))
                {
                    usedSkills.Add(skill.ID, skill);
                }
            }
            if (castLogsBySkill != null && castLogsBySkill.ContainsKey(skill))
            {
                isIndirectHealing = false;
            }
            long timeSpentCasting = 0, timeSpentCastingNoInterrupt = 0;
            int numberOfCast = 0, numberOfCastNoInterrupt = 0, timeWasted = 0, timeSaved = 0;
            long minTimeSpentCasting = 0, maxTimeSpentCasting = 0;
            if (!isIndirectHealing && castLogsBySkill != null && castLogsBySkill.TryGetValue(skill, out List<AbstractCastEvent> clList))
            {
                (timeSpentCasting, timeSpentCastingNoInterrupt, minTimeSpentCasting, maxTimeSpentCasting, numberOfCast, numberOfCastNoInterrupt, timeSaved, timeWasted) = DmgDistributionDto.GetCastValues(clList, phase);
                castLogsBySkill.Remove(skill);
            }
            object[] skillItem = {
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
                    isIndirectHealing ? 0 : numberOfCastNoInterrupt,
                };
            return skillItem;
        }

        public static EXTHealingStatsHealingDistributionDto BuildIncomingHealingDistData(ParsedEvtcLog log, AbstractSingleActor p, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new EXTHealingStatsHealingDistributionDto
            {
                Distribution = new List<object[]>()
            };
            EXTFinalIncomingHealingStat incomingHealingStats = p.EXTHealing.GetIncomingHealStats(null, log, phase.Start, phase.End);
            IReadOnlyList<EXTAbstractHealingEvent> healingLogs = p.EXTHealing.GetIncomingHealEvents(null, log, phase.Start, phase.End);
            var healingLogsBySkill = healingLogs.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            dto.ContributedHealing = incomingHealingStats.Healed;
            dto.ContributedDownedHealing = incomingHealingStats.DownedHealed;
            var conditionsById = log.StatisticsHelper.PresentConditions.ToDictionary(x => x.ID);
            foreach (KeyValuePair<SkillItem, List<EXTAbstractHealingEvent>> pair in healingLogsBySkill)
            {
                dto.Distribution.Add(GetHealingToItem(pair.Key, pair.Value, null, usedSkills, usedBuffs, log.Buffs, phase));
            }
            return dto;
        }


        private static List<object[]> BuildHealingDistBodyData(ParsedEvtcLog log, IReadOnlyList<AbstractCastEvent> casting, IReadOnlyList<EXTAbstractHealingEvent> healingLogs, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, PhaseData phase)
        {
            var list = new List<object[]>();
            var castLogsBySkill = casting.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            var healingLogsBySkill = healingLogs.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            var conditionsById = log.StatisticsHelper.PresentConditions.ToDictionary(x => x.ID);
            foreach (KeyValuePair<SkillItem, List<EXTAbstractHealingEvent>> pair in healingLogsBySkill)
            {
                list.Add(GetHealingToItem(pair.Key, pair.Value, castLogsBySkill, usedSkills, usedBuffs, log.Buffs, phase));
            }
            // non damaging
            /*foreach (KeyValuePair<SkillItem, List<AbstractCastEvent>> pair in castLogsBySkill)
            {
                if (healingLogsBySkill.ContainsKey(pair.Key))
                {
                    continue;
                }

                if (!usedSkills.ContainsKey(pair.Key.ID))
                {
                    usedSkills.Add(pair.Key.ID, pair.Key);
                }
                long timeCasting = 0;
                int casts = 0;
                int timeWasted = 0, timeSaved = 0;
                foreach (AbstractCastEvent cl in pair.Value)
                {
                    if (phase.InInterval(cl.Time))
                    {
                        casts++;
                        switch (cl.Status)
                        {
                            case AbstractCastEvent.AnimationStatus.Interrupted:
                                timeWasted += cl.SavedDuration;
                                break;

                            case AbstractCastEvent.AnimationStatus.Reduced:
                                timeSaved += cl.SavedDuration;
                                break;
                        }
                    }
                    timeCasting += Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start);
                }

                object[] skillData = {
                    false,
                    pair.Key.ID,
                    0,
                    -1,
                    0,
                    casts,
                    -timeWasted / 1000.0,
                    timeSaved / 1000.0,
                    0,
                    timeCasting
                };
                list.Add(skillData);
            }*/
            return list;
        }

        private static EXTHealingStatsHealingDistributionDto BuildHealingDistDataInternal(ParsedEvtcLog log, EXTFinalOutgoingHealingStat outgoingHealingStats, AbstractSingleActor p, AbstractSingleActor target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new EXTHealingStatsHealingDistributionDto();
            IReadOnlyList<AbstractCastEvent> casting = p.GetIntersectingCastEvents(log, phase.Start, phase.End);
            IReadOnlyList<EXTAbstractHealingEvent> healingLogs = p.EXTHealing.GetJustActorOutgoingHealEvents(target, log, phase.Start, phase.End);
            dto.ContributedHealing = outgoingHealingStats.ActorHealing;
            dto.ContributedDownedHealing = outgoingHealingStats.ActorDownedHealing;
            dto.TotalHealing = outgoingHealingStats.Healing;
            dto.TotalCasting = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start));
            dto.Distribution = BuildHealingDistBodyData(log, casting, healingLogs, usedSkills, usedBuffs, phase);
            return dto;
        }


        public static EXTHealingStatsHealingDistributionDto BuildFriendlyHealingDistData(ParsedEvtcLog log, AbstractSingleActor actor, AbstractSingleActor target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            EXTFinalOutgoingHealingStat outgoingHealingStats = actor.EXTHealing.GetOutgoingHealStats(target, log, phase.Start, phase.End);
            return BuildHealingDistDataInternal(log, outgoingHealingStats, actor, target, phase, usedSkills, usedBuffs);
        }

        private static EXTHealingStatsHealingDistributionDto BuildHealingDistDataMinionsInternal(ParsedEvtcLog log, EXTFinalOutgoingHealingStat outgoingHealingStats, Minions minions, AbstractSingleActor target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new EXTHealingStatsHealingDistributionDto();
            IReadOnlyList<AbstractCastEvent> casting = minions.GetIntersectingCastEvents(log, phase.Start, phase.End);
            IReadOnlyList<EXTAbstractHealingEvent> healingLogs = minions.EXTHealing.GetOutgoingHealEvents(target, log, phase.Start, phase.End);
            dto.ContributedHealing = healingLogs.Sum(x => x.HealingDone);
            dto.TotalHealing = outgoingHealingStats.Healing;
            dto.TotalCasting = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start));
            dto.Distribution = BuildHealingDistBodyData(log, casting, healingLogs, usedSkills, usedBuffs, phase);
            return dto;
        }

        public static EXTHealingStatsHealingDistributionDto BuildFriendlyMinionHealingDistData(ParsedEvtcLog log, AbstractSingleActor actor, Minions minions, AbstractSingleActor target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            EXTFinalOutgoingHealingStat outgoingHealingStats = actor.EXTHealing.GetOutgoingHealStats(target, log, phase.Start, phase.End);

            return BuildHealingDistDataMinionsInternal(log, outgoingHealingStats, minions, target, phase, usedSkills, usedBuffs);
        }
    }
}
