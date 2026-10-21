namespace GW2EIEvtcParser.EIData;

internal class GainComputerByStackCappedPlusConstant : GainComputerByStackCapped
{
    private readonly double ConstantGain = 0;
    public GainComputerByStackCappedPlusConstant(int cap, double constant) : base(cap)
    {
        ConstantGain = constant;
    }

    protected override double GetGain(double gainPerStack, int stack)
    {
        return base.GetGain(gainPerStack, stack) + ConstantGain;
    }
}
