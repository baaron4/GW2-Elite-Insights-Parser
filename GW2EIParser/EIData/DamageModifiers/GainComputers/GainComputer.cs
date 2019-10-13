namespace GW2EIParser.EIData
{
    public abstract class GainComputer
    {
        public bool Multiplier { get; protected set; }

        public abstract double ComputeGain(double gainPerStack, int stack);
    }
}
