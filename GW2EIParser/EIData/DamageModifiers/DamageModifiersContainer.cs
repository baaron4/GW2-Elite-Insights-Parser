using System.Collections.Generic;
using System.Linq;
using static GW2EIParser.EIData.DamageModifier;

namespace GW2EIParser.EIData
{
    public class DamageModifiersContainer
    {

        public Dictionary<GeneralHelper.Source, List<DamageModifier>> DamageModifiersPerSource { get; }

        public Dictionary<string, DamageModifier> DamageModifiersByName { get; }

        public DamageModifiersContainer(ulong build)
        {
            var currentDamageMods = new List<DamageModifier>();
            foreach (List<DamageModifier> boons in AllDamageModifiers)
            {
                currentDamageMods.AddRange(boons.Where(x => x.MaxBuild > build && build >= x.MinBuild));
            }
            DamageModifiersPerSource = currentDamageMods.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());
            DamageModifiersByName = currentDamageMods.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList().First());
        }

        public List<DamageModifier> GetModifiersPerProf(string prof)
        {
            var res = new List<DamageModifier>();
            List<GeneralHelper.Source> srcs = GeneralHelper.ProfToEnum(prof);
            foreach (GeneralHelper.Source src in srcs)
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
