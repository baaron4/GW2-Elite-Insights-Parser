using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class MechanicChartDataDto
    {
        [DataMember]
        public string symbol;

        [DataMember]
        public string size;

        [DataMember]
        public string color;

        [DataMember]
        public List<List<List<double>>> points;

        [DataMember(EmitDefaultValue = false)]
        public bool visible;
    }
}
