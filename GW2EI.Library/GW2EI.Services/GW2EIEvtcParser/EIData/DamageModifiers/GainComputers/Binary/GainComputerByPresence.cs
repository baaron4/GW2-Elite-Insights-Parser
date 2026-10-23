namespace GW2EIEvtcParser.EIData;

internal class GainComputerByPresence : GainComputerByAtLeastNStacksPresent
{
    public GainComputerByPresence() : base(1)
    {
    }
}
