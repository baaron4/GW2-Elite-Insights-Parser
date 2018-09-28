using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonBuffs
    {
        public JsonBuffs(int phaseCount)
        {
            Generation = new double[phaseCount];
            Overstack = new double[phaseCount];
            Presence = new double[phaseCount];
            Uptime = new double[phaseCount];
            StateOvertime = new double[phaseCount];
        }

        public double[] Uptime;
        public double[] Presence;
        public double[] Generation;
        public double[] Overstack;
        public double[] StateOvertime;
    }
}
