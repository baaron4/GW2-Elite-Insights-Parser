using System.Collections.Generic;
using GW2EIParser.EIData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class BuffDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool Stacking { get; set; }
        public bool Consumable { get; set; }
        public bool Enemy { get; set; }

        public static void AssembleBoons(ICollection<Buff> boons, Dictionary<string, BuffDto> dict)
        {
            foreach (Buff boon in boons)
            {
                dict["b" + boon.ID] = new BuffDto()
                {
                    Id = boon.ID,
                    Name = boon.Name,
                    Icon = boon.Link,
                    Stacking = (boon.Type == Buff.BuffType.Intensity),
                    Consumable = (boon.Nature == Buff.BuffNature.Consumable),
                    Enemy = (boon.Source == Buff.BuffSource.Enemy)
                };
            }
        }
    }
}
