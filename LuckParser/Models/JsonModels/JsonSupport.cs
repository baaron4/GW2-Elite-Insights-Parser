using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonSupport
    {
        public JsonSupport(int phaseCount)
        {
            condiCleanse = new int[phaseCount];
            condiCleanseTime = new double[phaseCount];
            resurrectTime = new double[phaseCount];
            resurrects = new int[phaseCount];
        }

        public int[] resurrects;
        public double[] resurrectTime;
        public int[] condiCleanse;
        public double[] condiCleanseTime;
    }
}
