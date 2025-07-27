namespace GW2EIEvtcParser.EIData;

public class AgentFacingAgentConnectorDescription : AgentFacingConnectorDescription
{
    public readonly int DstMasterID;
    internal AgentFacingAgentConnectorDescription(AgentFacingAgentConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        DstMasterID = connector.DstAgent.EnglobingAgentItem.UniqueID;
    }
}
