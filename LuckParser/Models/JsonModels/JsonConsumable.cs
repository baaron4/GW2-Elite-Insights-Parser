using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonConsumable
    {
        public int Stack;
        public int Duration;
        public long Time;
        public long Id;

        public JsonConsumable(ParseModels.Player.Consumable food)
        {
            Stack = food.Stack;
            Duration = food.Duration;
            Time = food.Time;
            Id = food.Item.ID;
        }
    }
}
