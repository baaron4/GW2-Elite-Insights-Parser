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
        public String name;

        [DataMember]
        public String description;

        [DataMember]
        public String symbol;

        [DataMember]
        public String color;

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
