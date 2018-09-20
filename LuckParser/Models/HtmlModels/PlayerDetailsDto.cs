using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class PlayerDetailsDto
    {
        [DataMember]
        public List<DmgDistributionDto> dmgDistributions;
        [DataMember]
        public List<DmgDistributionDto> dmgDistributionsBoss;
        [DataMember]
        public List<DmgDistributionDto> dmgDistributionsTaken;
        [DataMember]
        public List<List<double[]>> rotation;
        [DataMember]
        public List<List<BoonChartDataDto>> boonGraph;

        [DataMember] public List<List<FoodDto>> food;
    }
}
