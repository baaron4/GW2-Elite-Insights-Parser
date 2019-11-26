using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Builders.HtmlModels
{
    public class DmgDistributionDto
    {
        public long ContributedDamage { get; set; }
        public long ContributedShieldDamage { get; set; }
        public long TotalDamage { get; set; }
        public List<object[]> Distribution { get; set; }

        public static object[] GetDMGDtoItem(KeyValuePair<SkillItem, List<AbstractDamageEvent>> entry, Dictionary<SkillItem, List<AbstractCastEvent>> castLogsBySkill, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBoons, BuffsContainer boons)
        {
            int totaldamage = 0,
                    mindamage = int.MaxValue,
                    maxdamage = int.MinValue,
                    hits = 0,
                    crit = 0,
                    flank = 0,
                    glance = 0,
                    shieldDamage = 0;
            bool IsIndirectDamage = false;
            foreach (AbstractDamageEvent dl in entry.Value.Where(x => !x.HasDowned))
            {
                IsIndirectDamage = IsIndirectDamage || dl is NonDirectDamageEvent;
                int curdmg = dl.Damage;
                totaldamage += curdmg;
                if (curdmg < mindamage) { mindamage = curdmg; }
                if (curdmg > maxdamage) { maxdamage = curdmg; }
                hits++;
                if (dl.HasCrit)
                {
                    crit++;
                }

                if (dl.HasGlanced)
                {
                    glance++;
                }

                if (dl.IsFlanking)
                {
                    flank++;
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

            int casts = 0, timeswasted = 0, timessaved = 0;
            if (!IsIndirectDamage && castLogsBySkill != null && castLogsBySkill.TryGetValue(entry.Key, out List<AbstractCastEvent> clList))
            {

                casts = clList.Count;
                foreach (AbstractCastEvent cl in clList)
                {
                    if (cl.Interrupted)
                    {
                        timeswasted += cl.ActualDuration;
                    }
                    else if (cl.ReducedAnimation && cl.ActualDuration < cl.ExpectedDuration)
                    {
                        timessaved += cl.ExpectedDuration - cl.ActualDuration;
                    }
                }
            }
            object[] skillItem = {
                    IsIndirectDamage,
                    entry.Key.ID,
                    totaldamage,
                    mindamage == int.MaxValue ? 0 : mindamage,
                    maxdamage == int.MinValue ? 0 : maxdamage,
                    IsIndirectDamage ? 0 : casts,
                    hits,
                    IsIndirectDamage ? 0 : crit,
                    IsIndirectDamage ? 0 : flank,
                    IsIndirectDamage ? 0 : glance,
                    IsIndirectDamage ? 0 : timeswasted / 1000.0,
                    IsIndirectDamage ? 0 : timessaved / 1000.0,
                    shieldDamage,
                };
            return skillItem;
        }
    }
}
