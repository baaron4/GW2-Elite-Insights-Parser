using System;

namespace GW2EIEvtcParser.EIData
{
    internal class GainComputerByStackMultiplier : GainComputer
    {
        public GainComputerByStackMultiplier()
        {
            Multiplier = true;
        }

        public override double ComputeGain(double gainPerStack, int stack)
        {
            var pow = Math.Pow(gainPerStack, stack);
            return pow / (100 + pow);
        }
    }
}
