using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class DmgDistributionDto
    {
        [DataMember]
        public long contributedDamage;
        [DataMember]
        public long totalDamage;
        [DataMember]
        public List<double[]> distribution;
    }
}
