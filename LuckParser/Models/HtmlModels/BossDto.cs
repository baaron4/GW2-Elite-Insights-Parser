using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class BossDto
    {
        [DataMember] public long id;
        [DataMember] public string name;
        [DataMember] public string icon;
        [DataMember] public long health;
        [DataMember] public readonly List<MinionDto> minions = new List<MinionDto>();

        public BossDto() { }

        public BossDto(long id, string name, string icon)
        {
            this.id = id;
            this.name = name;
            this.icon = icon;
        }
    }
}
