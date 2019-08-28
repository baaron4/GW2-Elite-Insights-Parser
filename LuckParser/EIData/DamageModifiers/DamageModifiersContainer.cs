using System.Collections.Generic;
using System.Linq;
using static LuckParser.EIData.DamageModifier;

namespace LuckParser.EIData
{
    public class DamageModifiersContainer
    {

        public Dictionary<ModifierSource, List<DamageModifier>> DamageModifiersPerSource { get; }

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


        private static List<ModifierSource> ProfToEnum(string prof)
        {
            return prof switch
            {
                "Druid" => new List<ModifierSource> { ModifierSource.Ranger, ModifierSource.Druid },
                "Soulbeast" => new List<ModifierSource> { ModifierSource.Ranger, ModifierSource.Soulbeast },
                "Ranger" => new List<ModifierSource> { ModifierSource.Ranger },
                "Scrapper" => new List<ModifierSource> { ModifierSource.Engineer, ModifierSource.Scrapper },
                "Holosmith" => new List<ModifierSource> { ModifierSource.Engineer, ModifierSource.Holosmith },
                "Engineer" => new List<ModifierSource> { ModifierSource.Engineer },
                "Daredevil" => new List<ModifierSource> { ModifierSource.Thief, ModifierSource.Daredevil },
                "Deadeye" => new List<ModifierSource> { ModifierSource.Thief, ModifierSource.Deadeye },
                "Thief" => new List<ModifierSource> { ModifierSource.Thief },
                "Weaver" => new List<ModifierSource> { ModifierSource.Elementalist, ModifierSource.Weaver },
                "Tempest" => new List<ModifierSource> { ModifierSource.Elementalist, ModifierSource.Tempest },
                "Elementalist" => new List<ModifierSource> { ModifierSource.Elementalist },
                "Mirage" => new List<ModifierSource> { ModifierSource.Mesmer, ModifierSource.Mirage },
                "Chronomancer" => new List<ModifierSource> { ModifierSource.Mesmer, ModifierSource.Chronomancer },
                "Mesmer" => new List<ModifierSource> { ModifierSource.Mesmer },
                "Scourge" => new List<ModifierSource> { ModifierSource.Necromancer, ModifierSource.Scourge },
                "Reaper" => new List<ModifierSource> { ModifierSource.Necromancer, ModifierSource.Reaper },
                "Necromancer" => new List<ModifierSource> { ModifierSource.Necromancer },
                "Spellbreaker" => new List<ModifierSource> { ModifierSource.Warrior, ModifierSource.Spellbreaker },
                "Berserker" => new List<ModifierSource> { ModifierSource.Warrior, ModifierSource.Berserker },
                "Warrior" => new List<ModifierSource> { ModifierSource.Warrior },
                "Firebrand" => new List<ModifierSource> { ModifierSource.Guardian, ModifierSource.Firebrand },
                "Dragonhunter" => new List<ModifierSource> { ModifierSource.Guardian, ModifierSource.Dragonhunter },
                "Guardian" => new List<ModifierSource> { ModifierSource.Guardian },
                "Renegade" => new List<ModifierSource> { ModifierSource.Revenant, ModifierSource.Renegade },
                "Herald" => new List<ModifierSource> { ModifierSource.Revenant, ModifierSource.Herald },
                "Revenant" => new List<ModifierSource> { ModifierSource.Revenant },
                _ => new List<ModifierSource> { },
            };
        }

        public List<DamageModifier> GetModifiersPerProf(string prof)
        {
            var res = new List<DamageModifier>();
            List<ModifierSource> srcs = ProfToEnum(prof);
            foreach (ModifierSource src in srcs)
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
