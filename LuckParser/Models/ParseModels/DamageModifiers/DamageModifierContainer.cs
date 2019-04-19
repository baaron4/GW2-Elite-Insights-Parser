using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.ParseModels.DamageModifier;
using static LuckParser.Models.Statistics;

namespace LuckParser.Models.ParseModels
{
    public class DamageModifierContainer
    {

        public Dictionary<ModifierSource, List<DamageModifier>> DamageModifiersPerSource { get; }

        public Dictionary<string, DamageModifier> DamageModifiersByName { get; } 

        public DamageModifierContainer(ulong build)
        {
            IEnumerable<DamageModifier> currentDamageMods = AllDamageModifiers.Where(x => x.MaxBuild >= build);
            DamageModifiersPerSource = currentDamageMods.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());
            DamageModifiersByName = currentDamageMods.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList().First());
        }


        private List<ModifierSource> ProfToEnum(string prof)
        {
            switch (prof)
            {
                case "Druid":
                    return new List<ModifierSource> { ModifierSource.Ranger, ModifierSource.Druid };
                case "Soulbeast":
                    return new List<ModifierSource> { ModifierSource.Ranger, ModifierSource.Soulbeast };
                case "Ranger":
                    return new List<ModifierSource> { ModifierSource.Ranger };
                case "Scrapper":
                    return new List<ModifierSource> { ModifierSource.Engineer, ModifierSource.Scrapper };
                case "Holosmith":
                    return new List<ModifierSource> { ModifierSource.Engineer, ModifierSource.Holosmith };
                case "Engineer":
                    return new List<ModifierSource> { ModifierSource.Engineer };
                case "Daredevil":
                    return new List<ModifierSource> { ModifierSource.Thief, ModifierSource.Daredevil };
                case "Deadeye":
                    return new List<ModifierSource> { ModifierSource.Thief, ModifierSource.Deadeye };
                case "Thief":
                    return new List<ModifierSource> { ModifierSource.Thief };
                case "Weaver":
                    return new List<ModifierSource> { ModifierSource.Elementalist, ModifierSource.Weaver };
                case "Tempest":
                    return new List<ModifierSource> { ModifierSource.Elementalist, ModifierSource.Tempest };
                case "Elementalist":
                    return new List<ModifierSource> { ModifierSource.Elementalist };
                case "Mirage":
                    return new List<ModifierSource> { ModifierSource.Mesmer, ModifierSource.Mirage };
                case "Chronomancer":
                    return new List<ModifierSource> { ModifierSource.Mesmer, ModifierSource.Chronomancer };
                case "Mesmer":
                    return new List<ModifierSource> { ModifierSource.Mesmer };
                case "Scourge":
                    return new List<ModifierSource> { ModifierSource.Necromancer, ModifierSource.Scourge };
                case "Reaper":
                    return new List<ModifierSource> { ModifierSource.Necromancer, ModifierSource.Reaper };
                case "Necromancer":
                    return new List<ModifierSource> { ModifierSource.Necromancer };
                case "Spellbreaker":
                    return new List<ModifierSource> { ModifierSource.Warrior, ModifierSource.Spellbreaker };
                case "Berserker":
                    return new List<ModifierSource> { ModifierSource.Warrior, ModifierSource.Berserker };
                case "Warrior":
                    return new List<ModifierSource> { ModifierSource.Warrior };
                case "Firebrand":
                    return new List<ModifierSource> { ModifierSource.Guardian, ModifierSource.Firebrand };
                case "Dragonhunter":
                    return new List<ModifierSource> { ModifierSource.Guardian, ModifierSource.Dragonhunter };
                case "Guardian":
                    return new List<ModifierSource> { ModifierSource.Guardian };
                case "Renegade":
                    return new List<ModifierSource> { ModifierSource.Revenant, ModifierSource.Renegade };
                case "Herald":
                    return new List<ModifierSource> { ModifierSource.Revenant, ModifierSource.Herald };
                case "Revenant":
                    return new List<ModifierSource> { ModifierSource.Revenant };
            }
            return new List<ModifierSource> { };
        }

        public List<DamageModifier> GetModifiersPerProf(string prof)
        {
            List<DamageModifier> res = new List<DamageModifier>();
            List<ModifierSource> srcs = ProfToEnum(prof);
            foreach (ModifierSource src in srcs)
            {
                if (DamageModifiersPerSource.TryGetValue(src, out var list))
                {
                    res.AddRange(list);
                }
            }
            return res;
        }
    }
}
