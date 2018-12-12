using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonStatsAll : JsonStats
    {
        public JsonStatsAll(int phaseCount) : base(phaseCount)
        {
            saved = new int[phaseCount];
            stackDist = new double[phaseCount];
            avgBoons = new double[phaseCount];
            avgConditions = new double[phaseCount];
            swapCount = new int[phaseCount];
            timeSaved = new double[phaseCount];
            timeWasted = new double[phaseCount];
            wasted = new int[phaseCount];
        }
        
        // Rates
        public int[] wasted;
        public double[] timeWasted;
        public int[] saved;
        public double[] timeSaved;
        public double[] stackDist;

        // boons
        public double[] avgBoons;
        public double[] avgConditions;

        // Counts
        public int[] swapCount;
    }
}
