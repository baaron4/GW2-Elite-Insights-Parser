namespace GW2EIEvtcParser.EIData;

internal class GainComputerByAtLeastNStacksPresent : GainComputerBinary
{
    public GainComputerByAtLeastNStacksPresent(int expectedStacks) : base(expectedStacks)
    {
    }

    protected override bool IsNumberOfStacksValid(int stack)
    {
        return stack >= ExpectedStacks;
    }
}
