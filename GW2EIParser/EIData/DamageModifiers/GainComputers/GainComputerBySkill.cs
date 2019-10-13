namespace GW2EIParser.EIData
{
    public class GainComputerBySkill : GainComputer
    {
        public GainComputerBySkill()
        {
            Multiplier = true;
        }

        public override double ComputeGain(double gainPerStack, int stack)
        {
            return stack > 0 ? 1.0 : 0.0;
        }
    }
}
