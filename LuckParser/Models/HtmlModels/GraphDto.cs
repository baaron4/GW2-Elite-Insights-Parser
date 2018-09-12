using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class GraphDto
    {
        [DataMember]
        public string id;
        [DataMember]
        public string name;

        public GraphDto() { }

        public GraphDto(string id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
