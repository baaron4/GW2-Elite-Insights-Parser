namespace GW2EIEvtcParser.ParsedData;

internal abstract class EffectEndEvent : StatusEvent
{

    /// <summary>
    /// Unique id for tracking a created effect.
    /// </summary>
    protected long TrackingID;
    internal EffectEndEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
    }
}
