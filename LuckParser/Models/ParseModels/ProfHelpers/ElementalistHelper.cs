using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class ElementalistHelper : ProfHelper
    {

        public static void RemoveDualBuffs(Player p, Dictionary<ushort, List<CombatItem>> buffsPerDst)
        {
            HashSet<long> duals = new HashSet<long>
            {
                WeaverHelper.FireDual,
                WeaverHelper.WaterDual,
                WeaverHelper.AirDual,
                WeaverHelper.EarthDual,
            };
            foreach (CombatItem c in buffsPerDst[p.InstID].Where(x => x.Time <= p.LastAware && x.Time >= p.FirstAware && duals.Contains(x.SkillID)))
            {
                c.OverrideSkillID(Boon.NoBuff);
            }
        }
    }
}
