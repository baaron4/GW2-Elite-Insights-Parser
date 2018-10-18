using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class BoonChartDataDto
    {
        [DataMember] public string name;
        [DataMember] public string color;
        [DataMember(EmitDefaultValue = false)] public bool visible;
        [DataMember] public List<double[]> data;
    }
}
