namespace GW2EIEvtcParser.EIData;

internal class GainComputerByAtMostNStacksPresent : GainComputer
{
    private readonly int ExpectedStacks;
    public GainComputerByAtMostNStacksPresent(int expectedStacks)
    {
        Multiplier = true;
        ExpectedStacks = expectedStacks;
    }

    public override double ComputeGain(double gainPerStack, int stack)
    {
        return stack <= ExpectedStacks ? gainPerStack * stack / (100 + stack * gainPerStack) : 0;
    }
}
