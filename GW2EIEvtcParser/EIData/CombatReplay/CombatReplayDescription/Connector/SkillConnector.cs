namespace GW2EIEvtcParser.EIData;

public class SkillConnectorDescription : ConnectorDescription
{
    public readonly long OwnerID;

    internal SkillConnectorDescription(SkillConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        var agent = connector.Agent;
        OwnerID = agent.EnglobingAgentItem.UniqueID;
    }
}
