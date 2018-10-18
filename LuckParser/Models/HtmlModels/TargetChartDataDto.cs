using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class TargetChartDataDto
    {
        [DataMember]
        public List<int> total;
        [DataMember]
        public double[] health;
    }
}
