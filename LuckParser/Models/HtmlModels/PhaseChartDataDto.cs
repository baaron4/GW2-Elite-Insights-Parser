using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class PhaseChartDataDto
    {
        [DataMember]
        public List<PlayerChartDataDto> players;
        [DataMember]
        public PlayerChartDataDto boss;
        [DataMember]
        public double[] bossHealth;
    }
}
