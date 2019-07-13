namespace LuckParser.EIData
{
    public class GainComputerByStack : GainComputer
    {
        public GainComputerByStack()
        {
            Multiplier = true;
        }

        public override double ComputeGain(double gainPerStack, int stack)
        {
            return gainPerStack * stack / (100 + stack * gainPerStack);
        }
    }
}
