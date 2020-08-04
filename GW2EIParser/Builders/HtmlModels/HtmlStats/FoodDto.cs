using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class FoodDto
    {
        public double Time { get; set; }
        public double Duration { get; set; }
        public long Id { get; set; }
        public int Stack { get; set; }
        public bool Dimished { get; set; }

        private FoodDto(Consumable consume)
        {
            Time = consume.Time / 1000.0;
            Duration = consume.Duration / 1000.0;
            Stack = consume.Stack;
            Id = consume.Buff.ID;
            Dimished = (consume.Buff.ID == 46587 || consume.Buff.ID == 46668);
        }

        public static List<FoodDto> BuildPlayerFoodData(ParsedEvtcLog log, Player p, Dictionary<long, Buff> usedBuffs)
        {
            var list = new List<FoodDto>();
            List<Consumable> consume = p.GetConsumablesList(log, 0, log.FightData.FightEnd);

            foreach (Consumable entry in consume)
            {
                usedBuffs[entry.Buff.ID] = entry.Buff;
                list.Add(new FoodDto(entry));
            }

            return list;
        }
    }
}
