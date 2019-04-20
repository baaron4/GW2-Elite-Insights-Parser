using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.JsonModels.JsonStatistics;

namespace LuckParser.Models.JsonModels
{
    /// <summary>
    /// Class representing buffs on player actors
    /// </summary>
    public class JsonPlayerBuffs
    {
        /// <summary>
        /// Player buffs item
        /// </summary>
        public class JsonPlayerBuffsData
        {
            /// <summary>
            /// Uptime of the buff
            /// </summary>
            public double Uptime;
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
            /// <summary>
            /// Buff presence (intensity based only)
            /// </summary>
            public double Presence;

            public JsonPlayerBuffsData(Statistics.FinalBuffs stats, bool generation)
            {
                if (generation)
                {
                    Generation = stats.Generation;
                    Overstack = stats.Overstack;
                    Wasted = stats.Wasted;
                    UnknownExtended = stats.UnknownExtended;
                    Extended = stats.Extended;
                    ByExtension = stats.ByExtension;
                }
                else
                {
                    Uptime = stats.Uptime;
                    Presence = stats.Presence;
                }
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
        public List<JsonPlayerBuffsData> BuffData;
        /// <summary>
        /// Array of int[2] that represents the number of the given buff status \n
        /// Value[i][0] will be the time, value[i][1] will be the number of the buff present from value[i][0] to value[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public List<int[]> States;
    }
}
