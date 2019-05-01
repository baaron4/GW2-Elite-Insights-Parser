using LuckParser.Models.ParseModels;
using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{ 
    public class BoonDto
    {    
        public long Id;       
        public string Name;       
        public string Icon;       
        public bool Stacking;
        public bool Consumable;
        public bool Enemy;

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
