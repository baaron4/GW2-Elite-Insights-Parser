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
        public Double avg;

        [DataMember]
        public List<List<Object>> val = new List<List<Object>>();
    }
}
