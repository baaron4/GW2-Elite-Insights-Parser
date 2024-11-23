namespace GW2EIEvtcParser.EIData;

public class AgentFacingAgentConnectorDescription : AgentFacingConnectorDescription
{
    public readonly int DstMasterId;
    internal AgentFacingAgentConnectorDescription(AgentFacingAgentConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        DstMasterId = connector.DstAgent.UniqueID;
    }
}
