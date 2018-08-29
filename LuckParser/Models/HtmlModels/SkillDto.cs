using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class SkillDto
    {
        [DataMember(EmitDefaultValue = false)]
        public long id;
        [DataMember(EmitDefaultValue=false)]
        public string name;
        [DataMember(EmitDefaultValue = false)]
        public string icon;
        [DataMember]
        public bool aa;

        public SkillDto(long id, string name, string icon)
        {
            this.id = id;
            this.name = name;
            this.icon = icon;
        }
    }
}
