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
            CondiCleanseTime = new float[phaseCount];
            ResurrectTime = new float[phaseCount];
            Resurrects = new int[phaseCount];
        }

        public int[] Resurrects;
        public float[] ResurrectTime;
        public int[] CondiCleanse;
        public float[] CondiCleanseTime;
    }
}
