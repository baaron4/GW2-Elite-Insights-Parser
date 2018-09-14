using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class EnemyDto
    {
        [DataMember] public string name;

        public EnemyDto() { }

        public EnemyDto(string name)
        {
            this.name = name;
        }
    }
}
