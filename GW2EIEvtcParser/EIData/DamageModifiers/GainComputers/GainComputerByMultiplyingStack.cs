using System;

namespace GW2EIEvtcParser.EIData
{
    internal class GainComputerByMultiplyingStack : GainComputer
    {
        public GainComputerByMultiplyingStack()
        {
            Multiplier = true;
        }

        public override double ComputeGain(double gainPerStack, int stack)
        {
            double pow = 100.0 * Math.Pow(1.0 + gainPerStack / 100.0, stack) - 100.0;
            return pow / (100 + pow);
        }
    }
}
