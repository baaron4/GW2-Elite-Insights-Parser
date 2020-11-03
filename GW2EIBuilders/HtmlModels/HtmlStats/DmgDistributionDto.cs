﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels
{
    internal class DmgDistributionDto
    {
        public long ContributedDamage { get; internal set; }
        public long ContributedShieldDamage { get; internal set; }
        public long TotalDamage { get; internal set; }
        public long TotalCasting { get; internal set; }
        public List<object[]> Distribution { get; internal set; }

        private static object[] GetDMGDtoItem(KeyValuePair<SkillItem, List<AbstractDamageEvent>> entry, Dictionary<SkillItem, List<AbstractCastEvent>> castLogsBySkill, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBoons, BuffsContainer boons, PhaseData phase)
        {
            int totaldamage = 0,
                    mindamage = int.MaxValue,
                    maxdamage = int.MinValue,
                    hits = 0,
                    crit = 0,
                    critDamage = 0,
                    connectedHits = 0,
                    flank = 0,
                    glance = 0,
                    shieldDamage = 0;
            bool IsIndirectDamage = false;
            foreach (AbstractDamageEvent dl in entry.Value.Where(x => !x.DoubleProcHit))
            {
                IsIndirectDamage = IsIndirectDamage || dl is NonDirectDamageEvent;
                int curdmg = dl.Damage;
                totaldamage += curdmg;
                hits++;
                if (dl.HasHit)
                {
                    if (curdmg < mindamage) { mindamage = curdmg; }
                    if (curdmg > maxdamage) { maxdamage = curdmg; }
                    connectedHits++;
                    if (dl.HasCrit)
                    {
                        crit++;
                        critDamage += dl.Damage;
                    }
                    if (dl.HasGlanced)
                    {
                        glance++;
                    }

                    if (dl.IsFlanking)
                    {
                        flank++;
                    }
                }

                shieldDamage += dl.ShieldDamage;
            }
            if (IsIndirectDamage)
            {
                if (!usedBoons.ContainsKey(entry.Key.ID))
                {
                    if (boons.BuffsByIds.TryGetValue(entry.Key.ID, out Buff buff))
                    {
                        usedBoons.Add(buff.ID, buff);
                    }
                    else
                    {
                        SkillItem aux = entry.Key;
                        var auxBoon = new Buff(aux.Name, entry.Key.ID, aux.Icon);
                        usedBoons.Add(auxBoon.ID, auxBoon);
                    }
                }
            }
            else
            {
                if (!usedSkills.ContainsKey(entry.Key.ID))
                {
                    usedSkills.Add(entry.Key.ID, entry.Key);
                }
            }

            long timeCasting = 0;
            int casts = 0, timeWasted = 0, timeSaved = 0;
            if (!IsIndirectDamage && castLogsBySkill != null && castLogsBySkill.TryGetValue(entry.Key, out List<AbstractCastEvent> clList))
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
                    entry.Key.ID,
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
                    IsIndirectDamage ? 0 : timeCasting
                };
            return skillItem;
        }

        internal static DmgDistributionDto BuildDMGTakenDistData(ParsedEvtcLog log, AbstractSingleActor p, int phaseIndex, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new DmgDistributionDto
            {
                Distribution = new List<object[]>()
            };
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            List<AbstractDamageEvent> damageLogs = p.GetDamageTakenLogs(null, log, phase.Start, phase.End);
            var damageLogsBySkill = damageLogs.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            dto.ContributedDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.Damage) : 0;
            dto.ContributedShieldDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.ShieldDamage) : 0;
            var conditionsById = log.Statistics.PresentConditions.ToDictionary(x => x.ID);
            foreach (KeyValuePair<SkillItem, List<AbstractDamageEvent>> entry in damageLogsBySkill)
            {
                dto.Distribution.Add(GetDMGDtoItem(entry, null, usedSkills, usedBuffs, log.Buffs, phase));
            }
            return dto;
        }


        private static List<object[]> BuildDMGDistBodyData(ParsedEvtcLog log, List<AbstractCastEvent> casting, List<AbstractDamageEvent> damageLogs, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, int phaseIndex)
        {
            var list = new List<object[]>();
            var castLogsBySkill = casting.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            var damageLogsBySkill = damageLogs.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            var conditionsById = log.Statistics.PresentConditions.ToDictionary(x => x.ID);
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            foreach (KeyValuePair<SkillItem, List<AbstractDamageEvent>> entry in damageLogsBySkill)
            {
                list.Add(GetDMGDtoItem(entry, castLogsBySkill, usedSkills, usedBuffs, log.Buffs, phase));
            }
            // non damaging
            foreach (KeyValuePair<SkillItem, List<AbstractCastEvent>> entry in castLogsBySkill)
            {
                if (damageLogsBySkill.ContainsKey(entry.Key))
                {
                    continue;
                }

                if (!usedSkills.ContainsKey(entry.Key.ID))
                {
                    usedSkills.Add(entry.Key.ID, entry.Key);
                }
                long timeCasting = 0;
                int casts = 0;
                int timeWasted = 0, timeSaved = 0;
                foreach (AbstractCastEvent cl in entry.Value)
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
                    entry.Key.ID, 
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
                    timeCasting
                };
                list.Add(skillData);
            }
            return list;
        }

        private static DmgDistributionDto BuildDMGDistDataInternal(ParsedEvtcLog log, FinalDPS dps, AbstractSingleActor p, NPC target, int phaseIndex, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new DmgDistributionDto();
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            List<AbstractCastEvent> casting = p.GetIntersectingCastLogs(log, phase.Start, phase.End);
            List<AbstractDamageEvent> damageLogs = p.GetJustActorDamageLogs(target, log, phase.Start, phase.End);
            dto.ContributedDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.Damage) : 0;
            dto.ContributedShieldDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.ShieldDamage) : 0;
            dto.TotalDamage = dps.Damage;
            dto.TotalCasting = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start));
            dto.Distribution = BuildDMGDistBodyData(log, casting, damageLogs, usedSkills, usedBuffs, phaseIndex);
            return dto;
        }


        internal static DmgDistributionDto BuildPlayerDMGDistData(ParsedEvtcLog log, Player p, NPC target, int phaseIndex, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            FinalDPS dps = p.GetDPSTarget(log, phaseIndex, target);
            return BuildDMGDistDataInternal(log, dps, p, target, phaseIndex, usedSkills, usedBuffs);
        }


        internal static DmgDistributionDto BuildTargetDMGDistData(ParsedEvtcLog log, NPC target, int phaseIndex, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            FinalDPS dps = target.GetDPSAll(log, phaseIndex);
            return BuildDMGDistDataInternal(log ,dps, target, null, phaseIndex, usedSkills, usedBuffs);
        }

        private static DmgDistributionDto BuildDMGDistDataMinionsInternal(ParsedEvtcLog log, FinalDPS dps, Minions minions, NPC target, int phaseIndex, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new DmgDistributionDto();
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            List<AbstractCastEvent> casting = minions.GetIntersectingCastLogs(log, phase.Start, phase.End);
            List<AbstractDamageEvent> damageLogs = minions.GetDamageLogs(target, log, phase.Start, phase.End);
            dto.ContributedDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.Damage) : 0;
            dto.ContributedShieldDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.ShieldDamage) : 0;
            dto.TotalDamage = dps.Damage;
            dto.TotalCasting = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start));
            dto.Distribution = BuildDMGDistBodyData(log, casting, damageLogs, usedSkills, usedBuffs, phaseIndex);
            return dto;
        }

        internal static DmgDistributionDto BuildPlayerMinionDMGDistData(ParsedEvtcLog log, Player p, Minions minions, NPC target, int phaseIndex, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            FinalDPS dps = p.GetDPSTarget(log, phaseIndex, target);

            return BuildDMGDistDataMinionsInternal(log, dps, minions, target, phaseIndex, usedSkills, usedBuffs);
        }


        internal static DmgDistributionDto BuildTargetMinionDMGDistData(ParsedEvtcLog log, NPC target, Minions minions, int phaseIndex, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            FinalDPS dps = target.GetDPSAll(log, phaseIndex);
            return BuildDMGDistDataMinionsInternal(log, dps, minions, null, phaseIndex, usedSkills, usedBuffs);
        }
    }
}
