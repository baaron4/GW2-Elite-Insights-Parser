using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.JsonBuffs;

namespace LuckParser.Models
{
    public class JsonTargetBuffs
    {
        public JsonTargetBuffs(int phaseCount)
        {
            Uptime = new double[phaseCount];
            Presence = new double[phaseCount];
            Generated = new Dictionary<string, double>[phaseCount];
            Overstacked = new Dictionary<string, double>[phaseCount];
            States = new List<int[]>();
        }

        public double[] Uptime;
        public double[] Presence;
        public Dictionary<string, double>[] Generated;
        public Dictionary<string, double>[] Overstacked;
        public List<int[]> States;
    }

}
