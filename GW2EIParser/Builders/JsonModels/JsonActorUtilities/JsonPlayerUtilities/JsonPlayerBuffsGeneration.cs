using System.Collections.Generic;
using GW2EIEvtcParser.EIData;

namespace GW2EIParser.Builders.JsonModels
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
            /// <summary>
            /// Generation done
            /// </summary>
            public double Generation { get; set; }
            /// <summary>
            /// Generation with overstack
            /// </summary>
            public double Overstack { get; set; }
            /// <summary>
            /// Wasted generation
            /// </summary>
            public double Wasted { get; set; }
            /// <summary>
            /// Extension from unknown source
            /// </summary>
            public double UnknownExtended { get; set; }
            /// <summary>
            /// Generation done by extension
            /// </summary>
            public double ByExtension { get; set; }
            /// <summary>
            /// Buff extended 
            /// </summary>
            public double Extended { get; set; }

            public JsonBuffsGenerationData(FinalPlayerBuffs stats)
            {
                Generation = stats.Generation;
                Overstack = stats.Overstack;
                Wasted = stats.Wasted;
                UnknownExtended = stats.UnknownExtended;
                Extended = stats.Extended;
                ByExtension = stats.ByExtension;
            }

        }

        /// <summary>
        /// ID of the buff
        /// </summary>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; set; }
        /// <summary>
        /// Array of buff data \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonBuffsGenerationData"/>
        public List<JsonBuffsGenerationData> BuffData { get; set; }
    }
}
