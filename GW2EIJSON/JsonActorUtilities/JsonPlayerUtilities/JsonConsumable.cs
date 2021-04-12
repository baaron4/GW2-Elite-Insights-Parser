using Newtonsoft.Json;

namespace GW2EIJSON
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
        public int Stack { get; set; }
        [JsonProperty]
        /// <summary>
        /// Duration of the consumable
        /// </summary>
        public int Duration { get; set; }
        [JsonProperty]
        /// <summary>
        /// Time of application of the consumable
        /// </summary>
        public long Time { get; set; }
        [JsonProperty]
        /// <summary>
        /// ID of the consumable
        /// </summary>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; set; }

        [JsonConstructor]
        public JsonConsumable()
        {

        }
    }
}
