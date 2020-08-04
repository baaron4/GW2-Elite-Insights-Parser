namespace GW2EIEvtcParser.EIData
{
    public abstract class GainComputer
    {
        public bool Multiplier { get; protected set; }
        public bool SkillBased { get; protected set; } = false;

        public abstract double ComputeGain(double gainPerStack, int stack);
    }
}
