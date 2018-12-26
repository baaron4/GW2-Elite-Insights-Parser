using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonTargetBuffs
    {
        public JsonTargetBuffs(int phaseCount)
        {
            uptime = new double[phaseCount];
            presence = new double[phaseCount];
            generated = new Dictionary<string, double>[phaseCount];
            overstacked = new Dictionary<string, double>[phaseCount];
            unknownExtension = new Dictionary<string, double>[phaseCount];
            extension = new Dictionary<string, double>[phaseCount];
            wasted = new Dictionary<string, double>[phaseCount];
            states = new List<int[]>();
        }

        public double[] uptime;
        public double[] presence;
        public Dictionary<string, double>[] generated;
        public Dictionary<string, double>[] overstacked;
        public Dictionary<string, double>[] wasted;
        public Dictionary<string, double>[] unknownExtension;
        public Dictionary<string, double>[] extension;
        public List<int[]> states;
    }

}
