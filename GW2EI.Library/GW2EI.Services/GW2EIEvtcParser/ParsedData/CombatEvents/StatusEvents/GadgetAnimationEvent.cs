namespace GW2EIEvtcParser.ParsedData;

public class GadgetAnimationEvent : StatusEvent
{
    public readonly Token AnimationToken;

    public GadgetAnimationEvent? Next { get; private set; }

    public long? LoopEnd => Next?.Time;

    internal GadgetAnimationEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        AnimationToken = GetAnimationToken(evtcItem);
    }

    internal static Token GetAnimationToken(CombatItem evtcItem)
    {
        return new Token(evtcItem.DstAgent);
    }

    internal void SetNext(GadgetAnimationEvent next)
    {
        Next = next;
    }
}
