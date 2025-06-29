namespace GW2EIEvtcParser.EIData;

public class AgentConnectorDescription : GeographicalConnectorDescription
{
    public int MasterID { get; private set; }
    internal AgentConnectorDescription(AgentConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        MasterID = connector.Agent.UniqueID;
    }
}
