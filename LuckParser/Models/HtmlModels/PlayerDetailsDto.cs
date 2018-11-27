using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{   
    public class PlayerDetailsDto
    {
         public List<DmgDistributionDto> dmgDistributions;
         public List<List<DmgDistributionDto>> dmgDistributionsTargets;
         public List<DmgDistributionDto> dmgDistributionsTaken;
         public List<List<object[]>> rotation;
         public List<List<BoonChartDataDto>> boonGraph;
         public List<FoodDto> food;
         public List<PlayerDetailsDto> minions;
         public List<DeathRecapDto> deathRecap;
    }
}
