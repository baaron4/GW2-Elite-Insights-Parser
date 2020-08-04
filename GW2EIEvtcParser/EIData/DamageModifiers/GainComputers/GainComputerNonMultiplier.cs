namespace GW2EIEvtcParser.EIData
{
    public class GainComputerNonMultiplier : GainComputer
    {
        public GainComputerNonMultiplier()
        {
            Multiplier = false;
        }

        public override double ComputeGain(double gainPerStack, int stack)
        {
            return stack > 0 ? 1.0 : 0.0;
        }
    }
}
