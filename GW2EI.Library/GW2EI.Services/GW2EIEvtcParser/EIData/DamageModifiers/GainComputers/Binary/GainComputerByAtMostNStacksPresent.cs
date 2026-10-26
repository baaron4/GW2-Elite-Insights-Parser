namespace GW2EIEvtcParser.EIData;

internal class GainComputerByAtMostNStacksPresent : GainComputerBinary
{
    public GainComputerByAtMostNStacksPresent(int expectedStacks) : base(expectedStacks)
    {
    }

    protected override bool IsNumberOfStacksValid(int stack)
    {
        return stack <= ExpectedStacks;
    }
}
