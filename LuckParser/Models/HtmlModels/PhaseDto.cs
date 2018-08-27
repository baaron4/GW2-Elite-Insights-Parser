using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class PhaseDto
    {
        [DataMember]
        public string name;
        [DataMember]
        public long duration;
        [DataMember]
        public List<List<Object>> dpsStats;
        [DataMember]
        public List<List<Object>> dmgStatsBoss;
        [DataMember]
        public List<List<Object>> dmgStats;
        [DataMember]
        public List<List<Object>> defStats;
        [DataMember]
        public List<List<Object>> healStats;
        [DataMember]
        public List<BoonData> boonStats;
        [DataMember]
        public List<BoonData> offBuffStats;
        [DataMember]
        public List<BoonData> defBuffStats;

        public PhaseDto() { }

        public PhaseDto(string name, long duration)
        {
            this.name = name;
            this.duration = duration;
        }
    }
}
