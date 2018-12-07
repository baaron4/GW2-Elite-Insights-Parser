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
            uptime = new double[phaseCount];
            presence = new double[phaseCount];
            generated = new Dictionary<string, double>[phaseCount];
            overstacked = new Dictionary<string, double>[phaseCount];
            states = new List<int[]>();
        }

        public double[] uptime;
        public double[] presence;
        public Dictionary<string, double>[] generated;
        public Dictionary<string, double>[] overstacked;
        public List<int[]> states;
    }

}
