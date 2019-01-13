using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{   
    public class PlayerDetailsDto
    {
         public List<DmgDistributionDto> DmgDistributions;
         public List<List<DmgDistributionDto>> DmgDistributionsTargets;
         public List<DmgDistributionDto> DmgDistributionsTaken;
         public List<List<object[]>> Rotation;
         public List<List<BoonChartDataDto>> BoonGraph;
         public List<FoodDto> Food;
         public List<PlayerDetailsDto> Minions;
         public List<DeathRecapDto> DeathRecap;
    }
}
