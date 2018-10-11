using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class BossChartDataDto
    {
        [DataMember]
        public List<int> total;
        [DataMember]
        public double[] health;
    }
}
