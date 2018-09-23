using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class MinionDto
    {
        [DataMember] public long id;
        [DataMember] public string name;

        public MinionDto() { }

        public MinionDto(long id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
