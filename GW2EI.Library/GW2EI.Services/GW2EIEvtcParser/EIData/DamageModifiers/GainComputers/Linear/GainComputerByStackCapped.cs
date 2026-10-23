namespace GW2EIEvtcParser.EIData;

internal class GainComputerByStackCapped : GainComputerLinearByStack
{
    protected readonly int Cap;
    public GainComputerByStackCapped(int cap)
    {
        Cap = cap;
    }

    protected override double GetGain(double gainPerStack, int stack)
    {
        return gainPerStack * Math.Min(stack, Cap);
    }
}
