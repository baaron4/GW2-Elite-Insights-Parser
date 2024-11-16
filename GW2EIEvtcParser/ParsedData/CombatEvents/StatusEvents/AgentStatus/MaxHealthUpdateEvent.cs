namespace GW2EIEvtcParser.ParsedData;

public class MaxHealthUpdateEvent : StatusEvent
{
    public readonly int MaxHealth;

    internal MaxHealthUpdateEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        MaxHealth = GetMaxHealth(evtcItem);
    }

    internal static int GetMaxHealth(CombatItem evtcItem)
    {
        return (int)evtcItem.DstAgent;
    }

}
