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

        public static List<BoonDto> AssembleBoons(ICollection<Boon> boons)
        {
            List<BoonDto> dtos = new List<BoonDto>();
            foreach (Boon boon in boons)
            {
                dtos.Add(new BoonDto()
                {
                    Id = boon.ID,
                    Name = boon.Name,
                    Icon = boon.Link,
                    Stacking = (boon.Type == Boon.BoonType.Intensity),
                    Consumable = (boon.Nature == Boon.BoonNature.Consumable),
                    Enemy = (boon.Source == Boon.BoonSource.Enemy)
                });
            }
            return dtos;
        }
    }
}
