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

        [DataMember(EmitDefaultValue = false)]
        public bool enemyMech;

        [DataMember(EmitDefaultValue = false)]
        public bool playerMech;

        [DataMember(EmitDefaultValue = false)]
        public bool visible;

        [DataMember]
        public List<List<List<double>>> points;
    }
}
