using System.Collections.Generic;
using GW2EIParser.EIData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class DamageModDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Tooltip { get; set; }
        public bool NonMultiplier { get; set; }

        public static void AssembleDamageModifiers(ICollection<DamageModifier> damageMods, Dictionary<string, DamageModDto> dict)
        {
            foreach (DamageModifier mod in damageMods)
            {
                int id = mod.Name.GetHashCode();
                dict["d" + id] = new DamageModDto()
                {
                    Id = id,
                    Name = mod.Name,
                    Icon = mod.Icon,
                    Tooltip = mod.Tooltip,
                    NonMultiplier = !mod.Multiplier
                };
            }
        }
    }
}
