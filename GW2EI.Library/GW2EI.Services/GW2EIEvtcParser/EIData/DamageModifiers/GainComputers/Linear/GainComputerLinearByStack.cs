namespace GW2EIEvtcParser.EIData;

internal abstract class GainComputerLinearByStack : GainComputer
{
    protected GainComputerLinearByStack()
    {
    }

    protected abstract double GetGain(double gainPerStack, int stack);

    public override double ComputeGain(double gainPerStack, int stack)
    {
        var gain = GetGain(gainPerStack, stack);
        return gain / (100 + gain);
    }
}
