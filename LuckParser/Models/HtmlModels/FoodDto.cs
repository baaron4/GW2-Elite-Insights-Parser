using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class FoodDto
    {
        [DataMember] public double time;
        [DataMember] public double duration;
        [DataMember] public string icon;
        [DataMember] public string name;
        [DataMember] public int stack;
        [DataMember] public bool dimished;
    }
}
