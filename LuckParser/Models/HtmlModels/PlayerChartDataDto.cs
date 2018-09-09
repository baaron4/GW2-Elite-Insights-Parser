using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class PlayerChartDataDto
    {
        [DataMember]
        public List<int> boss;
        [DataMember]
        public List<int> cleave;
    }
}
