using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class PlayerChartDataDto
    {
        [DataMember]
        public List<List<int>> targets;
        [DataMember]
        public List<int> total;
    }
}
