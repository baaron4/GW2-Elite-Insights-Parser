namespace GW2EIEvtcParser.ParsedData;

public class AnimationSpeedEvent : StatusEvent
{
    public readonly float Speed;

    internal AnimationSpeedEvent(CombatItem combatItem, AgentData agentData) : base(combatItem, agentData)
    {
        Speed = BitConverter.Int32BitsToSingle(combatItem.Value);
    }
}
