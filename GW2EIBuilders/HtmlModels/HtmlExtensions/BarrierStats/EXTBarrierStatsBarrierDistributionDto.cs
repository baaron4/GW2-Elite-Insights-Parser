using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.EXTBarrier
{
    internal class EXTBarrierStatsBarrierDistributionDto
    {
        public long ContributedBarrier { get; set; }
        public long TotalBarrier { get; set; }
        public long TotalCasting { get; set; }
        public List<object[]> Distribution { get; set; }

        private static object[] GetBarrierToItem(SkillItem skill, List<EXTAbstractBarrierEvent> barrierLogs, Dictionary<SkillItem, List<AbstractCastEvent>> castLogsBySkill, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBoons, BuffsContainer boons, PhaseData phase)
        {
            int totalbarrier = 0,
                    minbarrier = int.MaxValue,
                    maxbarrier = int.MinValue,
                    hits = 0;
            bool isIndirectBarrier = false;
            foreach (EXTAbstractBarrierEvent dl in barrierLogs)
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
                isIndirectBarrier = false;
            }
            long timeSpentCasting = 0, timeSpentCastingNoInterrupt = 0;
            int numberOfCast = 0, numberOfCastNoInterrupt = 0, timeWasted = 0, timeSaved = 0;
            long minTimeSpentCasting = 0, maxTimeSpentCasting = 0;
            if (!isIndirectBarrier && castLogsBySkill != null && castLogsBySkill.TryGetValue(skill, out List<AbstractCastEvent> clList))
            {
                (timeSpentCasting, timeSpentCastingNoInterrupt, minTimeSpentCasting, maxTimeSpentCasting, numberOfCast, numberOfCastNoInterrupt, timeSaved, timeWasted) = DmgDistributionDto.GetCastValues(clList, phase);
                castLogsBySkill.Remove(skill);
            }
            object[] skillItem = {
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
                    isIndirectBarrier ? 0 : numberOfCastNoInterrupt,
                };
            return skillItem;
        }

        public static EXTBarrierStatsBarrierDistributionDto BuildIncomingBarrierDistData(ParsedEvtcLog log, AbstractSingleActor p, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new EXTBarrierStatsBarrierDistributionDto
            {
                Distribution = new List<object[]>()
            };
            EXTFinalIncomingBarrierStat incomingBarrierStats = p.EXTBarrier.GetIncomingBarrierStats(null, log, phase.Start, phase.End);
            IReadOnlyList<EXTAbstractBarrierEvent> barrierLogs = p.EXTBarrier.GetIncomingBarrierEvents(null, log, phase.Start, phase.End);
            var barrierLogsBySkill = barrierLogs.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            dto.ContributedBarrier = incomingBarrierStats.BarrierReceived;
            var conditionsById = log.StatisticsHelper.PresentConditions.ToDictionary(x => x.ID);
            foreach (KeyValuePair<SkillItem, List<EXTAbstractBarrierEvent>> pair in barrierLogsBySkill)
            {
                dto.Distribution.Add(GetBarrierToItem(pair.Key, pair.Value, null, usedSkills, usedBuffs, log.Buffs, phase));
            }
            return dto;
        }


        private static List<object[]> BuildBarrierDistBodyData(ParsedEvtcLog log, IReadOnlyList<AbstractCastEvent> casting, IReadOnlyList<EXTAbstractBarrierEvent> barrierLogs, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, PhaseData phase)
        {
            var list = new List<object[]>();
            var castLogsBySkill = casting.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            var barrierLogsBySkill = barrierLogs.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<SkillItem, List<EXTAbstractBarrierEvent>> pair in barrierLogsBySkill)
            {
                list.Add(GetBarrierToItem(pair.Key, pair.Value, castLogsBySkill, usedSkills, usedBuffs, log.Buffs, phase));
            }
            return list;
        }

        private static EXTBarrierStatsBarrierDistributionDto BuildBarrierDistDataInternal(ParsedEvtcLog log, EXTFinalOutgoingBarrierStat outgoingBarrierStats, AbstractSingleActor p, AbstractSingleActor target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new EXTBarrierStatsBarrierDistributionDto();
            IReadOnlyList<AbstractCastEvent> casting = p.GetIntersectingCastEvents(log, phase.Start, phase.End);
            IReadOnlyList<EXTAbstractBarrierEvent> barrierLogs = p.EXTBarrier.GetJustActorOutgoingBarrierEvents(target, log, phase.Start, phase.End);
            dto.ContributedBarrier = outgoingBarrierStats.ActorBarrier;
            dto.TotalBarrier = outgoingBarrierStats.Barrier;
            dto.TotalCasting = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start));
            dto.Distribution = BuildBarrierDistBodyData(log, casting, barrierLogs, usedSkills, usedBuffs, phase);
            return dto;
        }


        public static EXTBarrierStatsBarrierDistributionDto BuildFriendlyBarrierDistData(ParsedEvtcLog log, AbstractSingleActor actor, AbstractSingleActor target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            EXTFinalOutgoingBarrierStat outgoingBarrierStats = actor.EXTBarrier.GetOutgoingBarrierStats(target, log, phase.Start, phase.End);
            return BuildBarrierDistDataInternal(log, outgoingBarrierStats, actor, target, phase, usedSkills, usedBuffs);
        }

        private static EXTBarrierStatsBarrierDistributionDto BuildBarrierDistDataMinionsInternal(ParsedEvtcLog log, EXTFinalOutgoingBarrierStat outgoingBarrierStats, Minions minions, AbstractSingleActor target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new EXTBarrierStatsBarrierDistributionDto();
            IReadOnlyList<AbstractCastEvent> casting = minions.GetIntersectingCastEvents(log, phase.Start, phase.End);
            IReadOnlyList<EXTAbstractBarrierEvent> barrierLogs = minions.EXTBarrier.GetOutgoingBarrierEvents(target, log, phase.Start, phase.End);
            dto.ContributedBarrier = barrierLogs.Sum(x => x.BarrierGiven);
            dto.TotalBarrier = outgoingBarrierStats.Barrier;
            dto.TotalCasting = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start));
            dto.Distribution = BuildBarrierDistBodyData(log, casting, barrierLogs, usedSkills, usedBuffs, phase);
            return dto;
        }

        public static EXTBarrierStatsBarrierDistributionDto BuildFriendlyMinionBarrierDistData(ParsedEvtcLog log, AbstractSingleActor actor, Minions minions, AbstractSingleActor target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            EXTFinalOutgoingBarrierStat outgoingBarrierStats = actor.EXTBarrier.GetOutgoingBarrierStats(target, log, phase.Start, phase.End);

            return BuildBarrierDistDataMinionsInternal(log, outgoingBarrierStats, minions, target, phase, usedSkills, usedBuffs);
        }
    }
}
