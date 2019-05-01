using LuckParser.Models.ParseModels;
using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{ 
    public class DamageModDto
    {    
        public long Id;       
        public string Name;       
        public string Icon;
        public string Tooltip;
        public bool NonMultiplier;

        public static void AssembleDamageModifiers(ICollection<DamageModifier> damageMods, Dictionary<string, DamageModDto> dict)
        {
            foreach (DamageModifier mod in damageMods)
            {
                int id = mod.Name.GetHashCode();
                dict["d" + id] = new DamageModDto()
                {
                    Id = id,
                    Name = mod.Name,
                    Icon = mod.Url,
                    Tooltip = mod.Tooltip,
                    NonMultiplier = !mod.Multiplier
                };
            }
        }
    }
}
