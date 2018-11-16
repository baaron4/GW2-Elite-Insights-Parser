using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class DeathRecapDto
    {
        [DataMember] public long id;
        [DataMember] public long time;
        [DataMember] public List<object[]> toDown;
        [DataMember] public List<object[]> toKill;
    }
}
