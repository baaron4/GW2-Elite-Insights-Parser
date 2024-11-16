namespace GW2EIEvtcParser.EIData;

public abstract class AbstractCombatReplayDescription
{
    //TODO(Rennorb) use enum
    public string Type { get; protected set; }

    protected AbstractCombatReplayDescription()
    {
    }
}
