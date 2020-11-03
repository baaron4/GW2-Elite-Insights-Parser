using System.Collections.Generic;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class DamageModDto
    {
        public long Id { get; internal set; }
        public string Name { get; internal set; }
        public string Icon { get; internal set; }
        public string Tooltip { get; internal set; }
        public bool NonMultiplier { get; internal set; }
        public bool SkillBased { get; internal set; }

        internal static void AssembleDamageModifiers(ICollection<DamageModifier> damageMods, Dictionary<string, DamageModDto> dict)
        {
            foreach (DamageModifier mod in damageMods)
            {
                int id = mod.ID;
                dict["d" + id] = new DamageModDto()
                {
                    Id = id,
                    Name = mod.Name,
                    Icon = mod.Icon,
                    Tooltip = mod.Tooltip,
                    NonMultiplier = !mod.Multiplier,
                    SkillBased = mod.SkillBased
                };
            }
        }
    }
}
