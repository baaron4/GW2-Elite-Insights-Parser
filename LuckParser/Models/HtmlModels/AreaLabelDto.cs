using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class AreaLabelDto
    {
        [DataMember] public double start;
        [DataMember] public double end;
        [DataMember] public string label;
        [DataMember(EmitDefaultValue = false)] public bool highlight;
    }
}
