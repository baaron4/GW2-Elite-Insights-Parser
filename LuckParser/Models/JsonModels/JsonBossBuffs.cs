using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.JsonBuffs;

namespace LuckParser.Models
{
    public class JsonBossBuffs
    {
        public JsonBossBuffs(int phaseCount)
        {
            Uptime = new double[phaseCount];
            Presence = new double[phaseCount];
            Generated = new Dictionary<string, double>[phaseCount];
            Overstacked = new Dictionary<string, double>[phaseCount];
            States = new List<SimplifiedSegment>();
        }

        public double[] Uptime;
        public double[] Presence;
        public Dictionary<string, double>[] Generated;
        public Dictionary<string, double>[] Overstacked;
        public List<SimplifiedSegment> States;
    }

}
