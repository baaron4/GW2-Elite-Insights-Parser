using System.ComponentModel;

namespace LuckParser.Models.HtmlModels
{   
    public class FoodDto
    {
        [DefaultValue(null)]
        public double time;
        public double duration;
        public long id;
        public int stack;
        public bool dimished;
    }
}
