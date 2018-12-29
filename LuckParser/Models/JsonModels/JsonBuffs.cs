using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonBuffs
    {
        public JsonBuffs(int phaseCount)
        {
            generation = new double[phaseCount];
            overstack = new double[phaseCount];
            wasted = new double[phaseCount];
            unknownExtension = new double[phaseCount];
            extension = new double[phaseCount];
            extended = new double[phaseCount];
            presence = new double[phaseCount];
            uptime = new double[phaseCount];
            states = new List<int[]>();
        }

        public double[] uptime;
        public double[] presence;
        public double[] generation;
        public double[] wasted;
        public double[] unknownExtension;
        public double[] extension;
        public double[] extended;
        public double[] overstack;
        public List<int[]> states;
    }
}
