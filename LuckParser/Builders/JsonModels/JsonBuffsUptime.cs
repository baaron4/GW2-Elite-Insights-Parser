using System.Collections.Generic;
using LuckParser.Models;

namespace LuckParser.Builders.JsonModels
{
    /// <summary>
    /// Class representing buffs on player actors
    /// </summary>
    public class JsonBuffsUptime
    {
        /// <summary>
        /// Player buffs item
        /// </summary>
        public class JsonBuffsUptimeData
        {
            /// <summary>
            /// Uptime of the buff
            /// </summary>
            public double Uptime;
            /// <summary>
            /// Buff presence (intensity based only)
            /// </summary>
            public double Presence;

            public JsonBuffsUptimeData(Statistics.FinalBuffs stats)
            {
                Uptime = stats.Uptime;
                Presence = stats.Presence;
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
        /// <seealso cref="JsonBuffsUptimeData"/>
        public List<JsonBuffsUptimeData> BuffData;
        /// <summary>
        /// Array of int[2] that represents the number of the given buff status \n
        /// Value[i][0] will be the time, value[i][1] will be the number of the buff present from value[i][0] to value[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public List<int[]> States;
    }
}
