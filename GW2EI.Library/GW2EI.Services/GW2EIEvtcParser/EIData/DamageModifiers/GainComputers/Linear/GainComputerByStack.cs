namespace GW2EIEvtcParser.EIData;

internal class GainComputerByStack : GainComputerLinearByStack
{
    public GainComputerByStack()
    {
    }

    protected override double GetGain(double gainPerStack, int stack)
    {
        return gainPerStack * stack;
    }
}
