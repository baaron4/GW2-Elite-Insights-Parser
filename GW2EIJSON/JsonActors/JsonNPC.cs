using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GW2EIJSON
{
    /// <summary>
    /// Class representing an NPC
    /// </summary>
    public class JsonNPC : JsonActor
    {
        [JsonProperty]
        /// <summary>
        /// Game ID of the target
        /// </summary>
        public int Id { get; set; }
        [JsonProperty]
        /// <summary>
        /// Final health of the target
        /// </summary>
        public int FinalHealth { get; set; }
        [JsonProperty]
        /// <summary>
        /// % of health burned
        /// </summary>
        public double HealthPercentBurned { get; set; }
        [JsonProperty]
        /// <summary>
        /// Time at which target became active
        /// </summary>
        public int FirstAware { get; set; }
        [JsonProperty]
        /// <summary>
        /// Time at which target became inactive 
        /// </summary>
        public int LastAware { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of buff status
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public IReadOnlyList<JsonBuffsUptime> Buffs { get; set; }
        [JsonProperty]
        /// <summary>
        /// Array of double[2] that represents the breakbar percent of the actor \n
        /// Value[i][0] will be the time, value[i][1] will be breakbar % \n
        /// If i corresponds to the last element that means the breakbar did not change for the remainder of the fight \n
        /// </summary>
        public IReadOnlyList<IReadOnlyList<double>> BreakbarPercents { get; set; }

        [JsonConstructor]
        public JsonNPC()
        {

        }
    }
}
