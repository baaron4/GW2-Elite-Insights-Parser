using GW2EIParser.EIData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class FoodDto
    {
        public double Time { get; set; }
        public double Duration { get; set; }
        public long Id { get; set; }
        public int Stack { get; set; }
        public bool Dimished { get; set; }

        public FoodDto(Player.Consumable consume)
        {
            Time = consume.Time / 1000.0;
            Duration = consume.Duration / 1000.0;
            Stack = consume.Stack;
            Id = consume.Buff.ID;
            Dimished = (consume.Buff.ID == 46587 || consume.Buff.ID == 46668);
        }
    }
}
