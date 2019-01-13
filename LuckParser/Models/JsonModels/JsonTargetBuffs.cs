using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public List<JsonTargetBuffsData> Data;
        public List<int[]> States;
    }

}
