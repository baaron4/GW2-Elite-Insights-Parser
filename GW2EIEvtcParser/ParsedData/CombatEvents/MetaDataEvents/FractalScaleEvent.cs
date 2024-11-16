namespace GW2EIEvtcParser.ParsedData;

public class FractalScaleEvent : MetaDataEvent
{
    public readonly byte Scale;

    internal FractalScaleEvent(CombatItem evtcItem) : base(evtcItem)
    {
        Scale = (byte)evtcItem.SrcAgent;
    }

}
