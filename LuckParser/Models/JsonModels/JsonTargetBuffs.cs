using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.JsonModels.JsonStatistics;

namespace LuckParser.Models.JsonModels
{
    public class JsonTargetBuffs
    {

        public class JsonTargetBuffsData
        {
            public double Uptime;
            public double Presence;
            public Dictionary<string, double> Generated;
            public Dictionary<string, double> Overstacked;
            public Dictionary<string, double> Wasted;
            public Dictionary<string, double> UnknownExtension;
            public Dictionary<string, double> Extension;
            public Dictionary<string, double> Extended;


            private static Dictionary<string, double> ConvertKeys(Dictionary<ParseModels.Player, double> toConvert)
            {
                Dictionary<string, double> res = new Dictionary<string, double>();
                foreach (var pair in toConvert)
                {
                    res[pair.Key.Character] = pair.Value;
                }
                return res;
            }

            public JsonTargetBuffsData(Statistics.FinalTargetBuffs stats)
            {
                Uptime = stats.Uptime;
                Presence = stats.Presence;
                Generated = ConvertKeys(stats.Generated);
                Overstacked = ConvertKeys(stats.Overstacked);
                Wasted = ConvertKeys(stats.Wasted);
                UnknownExtension = ConvertKeys(stats.UnknownExtension);
                Extension = ConvertKeys(stats.Extension);
                Extended = ConvertKeys(stats.Extended);
            }
        }

        /// <summary>
        /// ID of the buff
        /// </summary>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long ID;
        /// <summary>
        /// Array of buff data \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonTargetBuffsData"/>
        public List<JsonTargetBuffsData> BuffData;
        /// <summary>
        /// Array of int[2] that represents the number of the given buff status \n
        /// Value[i][0] will be the time, value[i][1] will be the number of the buff present from value[i][0] to value[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public List<int[]> States;
    }

}
