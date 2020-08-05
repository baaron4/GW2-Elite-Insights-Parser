using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EncounterLogic;

namespace GW2EIEvtcParser.EIData
{
    public class DamageModifiersContainer
    {

        public Dictionary<ParseHelper.Source, List<DamageModifier>> DamageModifiersPerSource { get; }

        public Dictionary<string, DamageModifier> DamageModifiersByName { get; }

        public DamageModifiersContainer(ulong build, FightLogic.ParseMode mode)
        {
            var currentDamageMods = new List<DamageModifier>();
            foreach (List<DamageModifier> boons in DamageModifier.AllDamageModifiers)
            {
                currentDamageMods.AddRange(boons.Where(x => x.MaxBuild > build && build >= x.MinBuild && x.Keep(mode)));
            }
            DamageModifiersPerSource = currentDamageMods.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());
            DamageModifiersByName = currentDamageMods.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList().First());
        }

        public List<DamageModifier> GetModifiersPerProf(string prof)
        {
            var res = new List<DamageModifier>();
            List<ParseHelper.Source> srcs = ParseHelper.ProfToEnum(prof);
            foreach (ParseHelper.Source src in srcs)
            {
                if (DamageModifiersPerSource.TryGetValue(src, out List<DamageModifier> list))
                {
                    res.AddRange(list);
                }
            }
            return res;
        }
    }
}
