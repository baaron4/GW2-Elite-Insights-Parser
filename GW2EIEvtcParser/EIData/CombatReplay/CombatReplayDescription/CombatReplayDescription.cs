namespace GW2EIEvtcParser.EIData;

public abstract class CombatReplayDescription
{
    //TODO(Rennorb) use enum
    public string Type { get; protected set; }

    protected CombatReplayDescription()
    {
    }
}
