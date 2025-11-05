namespace GW2EIEvtcParser.EIData;

internal class GainComputerByExactNumberOfBuffsPresent : GainComputer
{
    private readonly int ExpectedBuffs = 0;
    public GainComputerByExactNumberOfBuffsPresent(int expectedCount)
    {
        Multiplier = true;
        ExpectedBuffs = expectedCount;
    }

    public override double ComputeGain(double gainPerStack, int stack)
    {
        return stack == ExpectedBuffs ? gainPerStack / (100 + gainPerStack) : 0;
    }
}
