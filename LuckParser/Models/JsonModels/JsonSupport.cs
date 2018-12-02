using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonSupport
    {
        public JsonSupport(int phaseCount)
        {
            CondiCleanse = new int[phaseCount];
            CondiCleanseTime = new double[phaseCount];
            ResurrectTime = new double[phaseCount];
            Resurrects = new int[phaseCount];
        }

        public int[] Resurrects;
        public double[] ResurrectTime;
        public int[] CondiCleanse;
        public double[] CondiCleanseTime;
    }
}
