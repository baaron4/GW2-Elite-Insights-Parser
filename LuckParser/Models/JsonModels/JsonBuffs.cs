using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.JsonModels.JsonStatistics;

namespace LuckParser.Models.JsonModels
{
    public class JsonBuffs
    {
        /// <summary>
        /// ID of the buff
        /// </summary>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long ID;
        /// <summary>
        /// Array of buff data
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonBuffsData"/>
        public List<JsonBuffsData> Data;
        /// <summary>
        /// Array of int[2] that represents the number of the given buff status
        /// Value[i][0] will be the time, value[i][1] will be the number of the buff present from value[i][0] to value[i+1][0]
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public List<int[]> States;
    }
}
