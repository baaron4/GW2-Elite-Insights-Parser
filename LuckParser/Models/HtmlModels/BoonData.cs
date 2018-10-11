using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class BoonData
    {
        [DataMember]
        public double avg;

        [DataMember]
        public List<List<object>> val = new List<List<object>>();
    }
}
