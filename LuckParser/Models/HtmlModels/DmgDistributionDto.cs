using LuckParser.Models.ParseModels;
using LuckParser.Parser;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.HtmlModels
{  
    public class DmgDistributionDto
    {     
        public long ContributedDamage;     
        public long TotalDamage;      
        public List<object[]> Distribution;

        public static object[] GetDMGDtoItem(KeyValuePair<long, List<AbstractDamageEvent>> entry, Dictionary<long, List<AbstractCastEvent>> castLogsBySkill, SkillData skillData, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Boon> usedBoons, BoonsContainer boons)
        {
            int totaldamage = 0,
                    mindamage = int.MaxValue,
                    maxdamage = int.MinValue,
                    hits = 0,
                    crit = 0,
                    flank = 0,
                    glance = 0;
            bool IsIndirectDamage = false;
            foreach (AbstractDamageEvent dl in entry.Value.Where(x => !x.HasDowned))
            {
                IsIndirectDamage = dl is NonDirectDamageEvent;
                int curdmg = dl.Damage;
                totaldamage += curdmg;
                if (curdmg < mindamage) { mindamage = curdmg; }
                if (curdmg > maxdamage) { maxdamage = curdmg; }
                hits++;
                if (dl.HasCrit) crit++;
                if (dl.HasGlanced) glance++;
                if (dl.IsFlanking) flank++;
            }
            if (IsIndirectDamage)
            {
                if (!usedBoons.ContainsKey(entry.Key))
                {
                    if (boons.BoonsByIds.TryGetValue(entry.Key, out Boon buff))
                    {
                        usedBoons.Add(buff.ID, buff);
                    }
                    else
                    {
                        SkillItem aux = skillData.Get(entry.Key);
                        Boon auxBoon = new Boon(aux.Name, entry.Key, aux.Icon);
                        usedBoons.Add(auxBoon.ID, auxBoon);
                    }
                }
            }
            else
            {
                if (!usedSkills.ContainsKey(entry.Key)) usedSkills.Add(entry.Key, skillData.Get(entry.Key));
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
                    entry.Key,
                    totaldamage,
                    mindamage == int.MaxValue ? 0 : mindamage,
                    maxdamage == int.MinValue ? 0 : maxdamage,
                    IsIndirectDamage ? 0 : casts,
                    hits,
                    IsIndirectDamage ? 0 : crit,
                    IsIndirectDamage ? 0 : flank,
                    IsIndirectDamage ? 0 : glance,
                    IsIndirectDamage ? 0 : timeswasted / 1000.0,
                    IsIndirectDamage ? 0 : timessaved / 1000.0
                };
            return skillItem;
        }
    }
}
