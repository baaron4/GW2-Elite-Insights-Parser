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
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public long id;
        [DataMember(EmitDefaultValue=false, Order = 1)]
        public string name;
        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string icon;
        [DataMember(EmitDefaultValue = false, Order = 3)]
        public bool aa;

        public SkillDto(long id, string name, string icon, bool aa)
        {
            this.id = id;
            this.name = name;
            this.icon = icon;
            this.aa = aa;
        }
    }
}
