using GW2EIParser.Models;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class representing consumables
    /// </summary>
    public class JsonConsumable
    {
        /// <summary>
        /// Number of stacks
        /// </summary>
        public int Stack { get; set; }
        /// <summary>
        /// Duration of the consumable
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// Time of application of the consumable
        /// </summary>
        public long Time { get; set; }
        /// <summary>
        /// ID of the consumable
        /// </summary>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; set; }

        public JsonConsumable(GeneralStatistics.Consumable food)
        {
            Stack = food.Stack;
            Duration = food.Duration;
            Time = food.Time;
            Id = food.Buff.ID;
        }
    }
}
