using System.Collections.Generic;
using LuckParser.Models;

namespace LuckParser.Builders.JsonModels
{
    /// <summary>
    /// Class representing buffs generation by player actors
    /// </summary>
    public class JsonBuffsGeneration
    {
        /// <summary>
        /// Player buffs generation item
        /// </summary>
        public class JsonBuffsGenerationData
        {
            /// <summary>
            /// Generation done
            /// </summary>
            public double Generation;
            /// <summary>
            /// Generation with overstack
            /// </summary>
            public double Overstack;
            /// <summary>
            /// Wasted generation
            /// </summary>
            public double Wasted;
            /// <summary>
            /// Extension from unknown source
            /// </summary>
            public double UnknownExtended;
            /// <summary>
            /// Generation done by extension
            /// </summary>
            public double ByExtension;
            /// <summary>
            /// Buff extended 
            /// </summary>
            public double Extended;

            public JsonBuffsGenerationData(Statistics.FinalBuffs stats)
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
        public long Id;
        /// <summary>
        /// Array of buff data \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsData"/>
        public List<JsonBuffsGenerationData> BuffData;
    }
}
