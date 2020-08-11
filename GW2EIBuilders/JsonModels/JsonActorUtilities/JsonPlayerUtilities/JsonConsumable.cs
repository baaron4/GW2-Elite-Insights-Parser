using GW2EIEvtcParser.EIData;
using Newtonsoft.Json;

namespace GW2EIBuilders.JsonModels
{
    /// <summary>
    /// Class representing consumables
    /// </summary>
    public class JsonConsumable
    {
        [JsonProperty]
        /// <summary>
        /// Number of stacks
        /// </summary>
        public int Stack { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Duration of the consumable
        /// </summary>
        public int Duration { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Time of application of the consumable
        /// </summary>
        public long Time { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// ID of the consumable
        /// </summary>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; internal set; }

        [JsonConstructor]
        internal JsonConsumable()
        {

        }

        internal JsonConsumable(Consumable food)
        {
            Stack = food.Stack;
            Duration = food.Duration;
            Time = food.Time;
            Id = food.Buff.ID;
        }
    }
}
