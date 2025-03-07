namespace GW2EIEvtcParser.ParsedData;

public abstract class BuffStackEvent : BuffEvent
{
    public uint BuffInstance { get; protected set; }

    internal BuffStackEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
    {
        To = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
        By = ParserHelper._unknownAgent;
    }

    /*internal override int CompareTo(AbstractBuffEvent abe)
    {
        if (abe is AbstractBuffApplyEvent)
        {
            return 1;
        }
        if (abe is AbstractBuffStackEvent)
        {
            return 0;
        }
        return -1;
    }*/
}

