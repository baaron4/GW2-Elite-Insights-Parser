namespace GW2EIEvtcParser.ParsedData;

public class CrowdControlEvent : SkillEvent
{
    //

    public int Duration { get; private set; }
    public int DefianceCalculation { get; private set; }

    internal CrowdControlEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
        Duration = evtcItem.Value;
        DefianceCalculation = evtcItem.Value + (int)evtcItem.OverstackValue;
    }
    internal override double GetValue()
    {
        return Duration;
    }
}
