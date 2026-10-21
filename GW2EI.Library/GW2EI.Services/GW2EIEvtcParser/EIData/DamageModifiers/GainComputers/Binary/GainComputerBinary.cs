namespace GW2EIEvtcParser.EIData;

internal abstract class GainComputerBinary : GainComputer
{
    protected readonly int ExpectedStacks;
    protected GainComputerBinary(int expectedStacks)
    {
        ExpectedStacks = expectedStacks;
    }

    protected abstract bool IsNumberOfStacksValid(int stack);

    public override double ComputeGain(double gainPerStack, int stack)
    {
        return IsNumberOfStacksValid(stack) ? gainPerStack / (100 + gainPerStack) : 0;
    }
}
