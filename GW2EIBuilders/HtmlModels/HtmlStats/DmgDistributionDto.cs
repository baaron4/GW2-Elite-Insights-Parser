using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels
{
    internal class DmgDistributionDto
    {
        public long ContributedDamage { get; set; }
        public double ContributedBreakbarDamage { get; set; }
        public long ContributedShieldDamage { get; set; }
        public long TotalDamage { get; set; }
        public double TotalBreakbarDamage { get; set; }
        public long TotalCasting { get; set; }
        public List<object[]> Distribution { get; set; }

        private static object[] GetDMGDtoItem(SkillItem skill, List<AbstractHealthDamageEvent> damageLogs, Dictionary<SkillItem, List<AbstractCastEvent>> castLogsBySkill, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBoons, BuffsContainer boons, PhaseData phase)
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
            foreach (AbstractHealthDamageEvent dl in damageLogs)
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

            long timeCasting = 0;
            int casts = 0, timeWasted = 0, timeSaved = 0;
            if (!IsIndirectDamage && castLogsBySkill != null && castLogsBySkill.TryGetValue(skill, out List<AbstractCastEvent> clList))
            {
                foreach (AbstractCastEvent cl in clList)
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
            }
            object[] skillItem = {
                    IsIndirectDamage,
                    skill.ID,
                    totaldamage,
                    mindamage == int.MaxValue ? 0 : mindamage,
                    maxdamage == int.MinValue ? 0 : maxdamage,
                    IsIndirectDamage ? 0 : casts,
                    connectedHits,
                    IsIndirectDamage ? 0 : crit,
                    IsIndirectDamage ? 0 : flank,
                    IsIndirectDamage ? 0 : glance,
                    IsIndirectDamage ? 0 : -timeWasted / 1000.0,
                    IsIndirectDamage ? 0 : timeSaved / 1000.0,
                    shieldDamage,
                    IsIndirectDamage ? 0 : critDamage,
                    hits,
                    IsIndirectDamage ? 0 : timeCasting,
                    againstMoving
                };
            return skillItem;
        }

        public static DmgDistributionDto BuildDMGTakenDistData(ParsedEvtcLog log, AbstractSingleActor p, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new DmgDistributionDto
            {
                Distribution = new List<object[]>()
            };
            FinalDefensesAll incomingDamageStats = p.GetDefenseStats(log, phase.Start, phase.End);
            IReadOnlyList<AbstractHealthDamageEvent> damageLogs = p.GetDamageTakenEvents(null, log, phase.Start, phase.End);
            var damageLogsBySkill = damageLogs.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            dto.ContributedDamage = incomingDamageStats.DamageTaken;
            dto.ContributedShieldDamage = incomingDamageStats.DamageBarrier;
            foreach (KeyValuePair<SkillItem, List<AbstractHealthDamageEvent>> pair in damageLogsBySkill)
            {
                dto.Distribution.Add(GetDMGDtoItem(pair.Key, pair.Value, null, usedSkills, usedBuffs, log.Buffs, phase));
            }
            return dto;
        }


        private static List<object[]> BuildDMGDistBodyData(ParsedEvtcLog log, IReadOnlyList<AbstractCastEvent> casting, IReadOnlyList<AbstractHealthDamageEvent> damageLogs, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, PhaseData phase)
        {
            var list = new List<object[]>();
            var castLogsBySkill = casting.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            var damageLogsBySkill = damageLogs.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            var conditionsById = log.StatisticsHelper.PresentConditions.ToDictionary(x => x.ID);
            foreach (KeyValuePair<SkillItem, List<AbstractHealthDamageEvent>> pair in damageLogsBySkill)
            {
                list.Add(GetDMGDtoItem(pair.Key, pair.Value, castLogsBySkill, usedSkills, usedBuffs, log.Buffs, phase));
            }
            // non damaging
            foreach (KeyValuePair<SkillItem, List<AbstractCastEvent>> pair in castLogsBySkill)
            {
                if (damageLogsBySkill.ContainsKey(pair.Key))
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
                    0,
                    0,
                    0,
                    0,
                    -timeWasted / 1000.0,
                    timeSaved / 1000.0,
                    0,
                    0,
                    0,
                    timeCasting,
                    0
                };
                list.Add(skillData);
            }
            return list;
        }

        private static DmgDistributionDto BuildDMGDistDataInternal(ParsedEvtcLog log, FinalDPS dps, AbstractSingleActor p, AbstractSingleActor target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new DmgDistributionDto();
            IReadOnlyList<AbstractCastEvent> casting = p.GetIntersectingCastEvents(log, phase.Start, phase.End);
            IReadOnlyList<AbstractHealthDamageEvent> damageLogs = p.GetJustActorDamageEvents(target, log, phase.Start, phase.End);
            dto.ContributedDamage = dps.ActorDamage;
            dto.ContributedShieldDamage = dps.ActorBarrierDamage;
            dto.ContributedBreakbarDamage = dps.ActorBreakbarDamage;
            dto.TotalDamage = dps.Damage;
            dto.TotalBreakbarDamage = dps.BreakbarDamage;
            dto.TotalCasting = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start));
            dto.Distribution = BuildDMGDistBodyData(log, casting, damageLogs, usedSkills, usedBuffs, phase);
            return dto;
        }


        public static DmgDistributionDto BuildFriendlyDMGDistData(ParsedEvtcLog log, AbstractSingleActor actor, AbstractSingleActor target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            FinalDPS dps = actor.GetDPSStats(target, log, phase.Start, phase.End);
            return BuildDMGDistDataInternal(log, dps, actor, target, phase, usedSkills, usedBuffs);
        }


        public static DmgDistributionDto BuildTargetDMGDistData(ParsedEvtcLog log, AbstractSingleActor npc, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            FinalDPS dps = npc.GetDPSStats(log, phase.Start, phase.End);
            return BuildDMGDistDataInternal(log, dps, npc, null, phase, usedSkills, usedBuffs);
        }

        private static DmgDistributionDto BuildDMGDistDataMinionsInternal(ParsedEvtcLog log, FinalDPS dps, Minions minions, AbstractSingleActor target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new DmgDistributionDto();
            IReadOnlyList<AbstractCastEvent> casting = minions.GetIntersectingCastEvents(log, phase.Start, phase.End);
            IReadOnlyList<AbstractHealthDamageEvent> damageLogs = minions.GetDamageEvents(target, log, phase.Start, phase.End);
            IReadOnlyList<AbstractBreakbarDamageEvent> brkDamageLogs = minions.GetBreakbarDamageEvents(target, log, phase.Start, phase.End);
            dto.ContributedDamage = damageLogs.Sum(x => x.HealthDamage);
            dto.ContributedShieldDamage = damageLogs.Sum(x => x.ShieldDamage);
            dto.ContributedBreakbarDamage = Math.Round(brkDamageLogs.Sum(x => x.BreakbarDamage), 1);
            dto.TotalDamage = dps.Damage;
            dto.TotalBreakbarDamage = dps.BreakbarDamage;
            dto.TotalCasting = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start));
            dto.Distribution = BuildDMGDistBodyData(log, casting, damageLogs, usedSkills, usedBuffs, phase);
            return dto;
        }

        public static DmgDistributionDto BuildFriendlyMinionDMGDistData(ParsedEvtcLog log, AbstractSingleActor actor, Minions minions, AbstractSingleActor target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            FinalDPS dps = actor.GetDPSStats(target, log, phase.Start, phase.End);

            return BuildDMGDistDataMinionsInternal(log, dps, minions, target, phase, usedSkills, usedBuffs);
        }


        public static DmgDistributionDto BuildTargetMinionDMGDistData(ParsedEvtcLog log, AbstractSingleActor target, Minions minions, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            FinalDPS dps = target.GetDPSStats(log, phase.Start, phase.End);
            return BuildDMGDistDataMinionsInternal(log, dps, minions, null, phase, usedSkills, usedBuffs);
        }
    }
}
