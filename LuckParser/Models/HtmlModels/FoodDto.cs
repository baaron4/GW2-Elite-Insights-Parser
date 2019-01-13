using System.ComponentModel;

namespace LuckParser.Models.HtmlModels
{   
    public class FoodDto
    {
        [DefaultValue(null)]
        public double Time;
        public double Duration;
        public long Id;
        public int Stack;
        public bool Dimished;
    }
}
