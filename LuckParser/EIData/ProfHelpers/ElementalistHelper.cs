using System.Collections.Generic;
using System.Linq;
using LuckParser.Parser.ParsedData;
using LuckParser.Parser.ParsedData.CombatEvents;

namespace LuckParser.EIData
{
    public class ElementalistHelper : ProfHelper
    {

        public static void RemoveDualBuffs(List<AbstractBuffEvent> buffsPerDst, SkillData skillData)
        {
            var duals = new HashSet<long>
            {
                FireDual,
                WaterDual,
                AirDual,
                EarthDual,
            };
            foreach (AbstractBuffEvent c in buffsPerDst.Where(x => duals.Contains(x.BuffID)))
            {
                c.Invalidate(skillData);
            }
        }
    }
}
