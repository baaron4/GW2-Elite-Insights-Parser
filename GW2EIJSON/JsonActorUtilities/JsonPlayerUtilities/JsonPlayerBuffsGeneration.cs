using System.Collections.Generic;
using Newtonsoft.Json;

namespace GW2EIJSON
{
    /// <summary>
    /// Class representing buffs generation by player actors
    /// </summary>
    public class JsonPlayerBuffsGeneration
    {
        /// <summary>
        /// Player buffs generation item
        /// </summary>
        public class JsonBuffsGenerationData
        {
            [JsonProperty]
            /// <summary>
            /// Generation done
            /// </summary>
            public double Generation { get; set; }
            [JsonProperty]
            /// <summary>
            /// Generation with overstack
            /// </summary>
            public double Overstack { get; set; }
            [JsonProperty]
            /// <summary>
            /// Wasted generation
            /// </summary>
            public double Wasted { get; set; }
            [JsonProperty]
            /// <summary>
            /// Extension from unknown source
            /// </summary>
            public double UnknownExtended { get; set; }
            [JsonProperty]
            /// <summary>
            /// Generation done by extension
            /// </summary>
            public double ByExtension { get; set; }
            [JsonProperty]
            /// <summary>
            /// Buff extended 
            /// </summary>
            public double Extended { get; set; }

            [JsonConstructor]
            internal JsonBuffsGenerationData()
            {

            }

        }

        [JsonProperty]
        /// <summary>
        /// ID of the buff
        /// </summary>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; set; }
        [JsonProperty]
        /// <summary>
        /// Array of buff data \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonBuffsGenerationData"/>
        public IReadOnlyList<JsonBuffsGenerationData> BuffData { get; set; }
    }
}
