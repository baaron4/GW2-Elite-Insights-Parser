using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class MechanicDto
    {
        [DataMember]
        public string name;

        [DataMember]
        public string shortName;

        [DataMember]
        public string description;

        [DataMember]
        public string symbol;

        [DataMember]
        public string color;

        [DataMember]
        public bool playerMech;

        [DataMember]
        public bool enemyMech;

        [DataMember(EmitDefaultValue = false)]
        public bool visible;

        [DataMember]
        public List<List<List<double>>> data;
    }
}
