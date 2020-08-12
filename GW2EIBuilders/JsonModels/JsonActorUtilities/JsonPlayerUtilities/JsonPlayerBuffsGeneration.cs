using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using Newtonsoft.Json;

namespace GW2EIBuilders.JsonModels
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
            public double Generation { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Generation with overstack
            /// </summary>
            public double Overstack { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Wasted generation
            /// </summary>
            public double Wasted { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Extension from unknown source
            /// </summary>
            public double UnknownExtended { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Generation done by extension
            /// </summary>
            public double ByExtension { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Buff extended 
            /// </summary>
            public double Extended { get; internal set; }

            [JsonConstructor]
            internal JsonBuffsGenerationData()
            {

            }

            internal JsonBuffsGenerationData(FinalPlayerBuffs stats)
            {
                Generation = stats.Generation;
                Overstack = stats.Overstack;
                Wasted = stats.Wasted;
                UnknownExtended = stats.UnknownExtended;
                Extended = stats.Extended;
                ByExtension = stats.ByExtension;
            }

        }

        [JsonProperty]
        /// <summary>
        /// ID of the buff
        /// </summary>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Array of buff data \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonBuffsGenerationData"/>
        public List<JsonBuffsGenerationData> BuffData { get; internal set; }
    }
}
