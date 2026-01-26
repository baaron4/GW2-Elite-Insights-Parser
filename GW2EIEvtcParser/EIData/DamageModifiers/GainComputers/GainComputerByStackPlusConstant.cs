namespace GW2EIEvtcParser.EIData;

internal class GainComputerByStackPlusConstant : GainComputerByStack
{
    private readonly double ConstantGain = 0;
    public GainComputerByStackPlusConstant(double constant) : base()
    {
        ConstantGain = constant;
    }

    public override double ComputeGain(double gainPerStack, int stack)
    {
        return (gainPerStack * stack + ConstantGain) / (100 + (stack * gainPerStack + ConstantGain));
    }
}
