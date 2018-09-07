using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class BoonDto
    {
        [DataMember(EmitDefaultValue = false)]
        public long id;
        [DataMember(EmitDefaultValue=false)]
        public string name;
        [DataMember(EmitDefaultValue = false)]
        public string icon;
        [DataMember]
        public bool stacking;

        public BoonDto(long id, string name, string icon, bool stacking)
        {
            this.id = id;
            this.name = name;
            this.icon = icon;
            this.stacking = stacking;
        }
    }
}
