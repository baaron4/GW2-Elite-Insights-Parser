using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class ChartDataDto
    {
        [DataMember]
        public List<PhaseChartDataDto> phases = new List<PhaseChartDataDto>();
        [DataMember]
        public List<MechanicChartDataDto> mechanics = new List<MechanicChartDataDto>();
    }
}
