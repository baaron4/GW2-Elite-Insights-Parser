using System.Collections.Generic;
using LuckParser.EIData;

namespace LuckParser.Builders.HtmlModels
{
    public class BoonDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool Stacking { get; set; }
        public bool Consumable { get; set; }
        public bool Enemy { get; set; }

        public static void AssembleBoons(ICollection<Buff> boons, Dictionary<string, BoonDto> dict)
        {
            foreach (Buff boon in boons)
            {
                dict["b" + boon.ID] = new BoonDto()
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
