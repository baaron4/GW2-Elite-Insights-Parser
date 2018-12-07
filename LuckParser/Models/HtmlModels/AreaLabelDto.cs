using System.ComponentModel;

namespace LuckParser.Models.HtmlModels
{
    public class AreaLabelDto
    {
        [DefaultValue(null)]
        public double start;
        [DefaultValue(null)]
        public double end;
        public string label;
        public int highlight;
    }
}
