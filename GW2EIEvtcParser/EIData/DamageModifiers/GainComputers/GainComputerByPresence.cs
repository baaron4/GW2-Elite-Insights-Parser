namespace GW2EIEvtcParser.EIData
{
    internal class GainComputerByPresence : GainComputer
    {
        public GainComputerByPresence()
        {
            Multiplier = true;
        }

        public override double ComputeGain(double gainPerStack, int stack)
        {
            return stack > 0 ? gainPerStack / (100 + gainPerStack) : 0;
        }
    }
}
