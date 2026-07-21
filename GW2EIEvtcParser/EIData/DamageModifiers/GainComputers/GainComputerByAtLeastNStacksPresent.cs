namespace GW2EIEvtcParser.EIData;

internal class GainComputerByAtLeastNStacksPresent : GainComputer
{
    private readonly int ExpectedStacks;
    public GainComputerByAtLeastNStacksPresent(int expectedStacks)
    {
        Multiplier = true;
        ExpectedStacks = expectedStacks;
    }

    public override double ComputeGain(double gainPerStack, int stack)
    {
        return stack >= ExpectedStacks ? gainPerStack / (100 + gainPerStack) : 0;
    }
}
