using LuckParser.Models;

namespace LuckParser.Builders.HtmlModels
{
    public class FoodDto
    {
        public double Time;
        public double Duration;
        public long Id;
        public int Stack;
        public bool Dimished;

        public FoodDto(Statistics.Consumable consume)
        {
            Time = consume.Time / 1000.0;
            Duration = consume.Duration / 1000.0;
            Stack = consume.Stack;
            Id = consume.Buff.ID;
            Dimished = (consume.Buff.ID == 46587 || consume.Buff.ID == 46668);
        }
    }
}
