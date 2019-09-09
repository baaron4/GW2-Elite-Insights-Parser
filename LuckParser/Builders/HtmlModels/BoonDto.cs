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

        public static void AssembleBoons(ICollection<Boon> boons, Dictionary<string, BoonDto> dict)
        {
            foreach (Boon boon in boons)
            {
                dict["b" + boon.ID] = new BoonDto()
                {
                    Id = boon.ID,
                    Name = boon.Name,
                    Icon = boon.Link,
                    Stacking = (boon.Type == Boon.BoonType.Intensity),
                    Consumable = (boon.Nature == Boon.BoonNature.Consumable),
                    Enemy = (boon.Source == Boon.BoonSource.Enemy)
                };
            }
        }
    }
}
