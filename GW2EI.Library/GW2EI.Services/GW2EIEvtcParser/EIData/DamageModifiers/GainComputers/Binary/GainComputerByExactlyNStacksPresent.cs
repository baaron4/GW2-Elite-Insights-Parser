namespace GW2EIEvtcParser.EIData;

internal class GainComputerByExactlyNStacksPresent : GainComputerBinary
{
    public GainComputerByExactlyNStacksPresent(int expectedStacks) : base(expectedStacks)
    {
    }

    protected override bool IsNumberOfStacksValid(int stack)
    {
        return stack == ExpectedStacks;
    }
}
