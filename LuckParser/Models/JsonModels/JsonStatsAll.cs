using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonStatsAll : JsonStats
    {
        public JsonStatsAll(int phaseCount) : base(phaseCount)
        {
            Dcd = new double[phaseCount];
            Died = new double[phaseCount];
            DodgeCount = new int[phaseCount];
            DownCount = new int[phaseCount];
            Saved = new int[phaseCount];
            StackDist = new double[phaseCount];
            AvgBoons = new double[phaseCount];
            AvgConditions = new double[phaseCount];
            SwapCount = new int[phaseCount];
            TimeSaved = new double[phaseCount];
            TimeWasted = new double[phaseCount];
            Wasted = new int[phaseCount];
        }

        // Rates
        public int[] Wasted;
        public double[] TimeWasted;
        public int[] Saved;
        public double[] TimeSaved;
        public double[] StackDist;

        // Boons
        public double[] AvgBoons;
        public double[] AvgConditions;

        // Counts
        public int[] SwapCount;
        public int[] DownCount;
        public int[] DodgeCount;

        // Misc
        public double[] Died;
        public double[] Dcd;
    }
}
