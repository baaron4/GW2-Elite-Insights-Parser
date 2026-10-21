namespace GW2EIEvtcParser.EIData;

internal class GainComputerByStackPlusConstant : GainComputerByStack
{
    private readonly double ConstantGain = 0;
    public GainComputerByStackPlusConstant(double constant) : base()
    {
        ConstantGain = constant;
    }

    protected override double GetGain(double gainPerStack, int stack)
    {
        return base.GetGain(gainPerStack, stack) + ConstantGain;
    }
}
