using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class ElementalistHelper : BoonHelper
    {

        public static void RemoveDualBuffs(Player p, Dictionary<ushort, List<CombatItem>> buffsPerDst)
        {
            HashSet<long> duals = new HashSet<long>
            {
                FireDual,
                WaterDual,
                AirDual,
                EarthDual,
            };
            foreach (CombatItem c in buffsPerDst[p.InstID].Where(x => x.Time <= p.LastAware && x.Time >= p.FirstAware && duals.Contains(x.SkillID)))
            {
                c.OverrideSkillID(NoBuff);
            }
        }
    }
}
