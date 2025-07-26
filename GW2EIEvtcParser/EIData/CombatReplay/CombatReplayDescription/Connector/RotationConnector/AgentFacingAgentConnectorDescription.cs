namespace GW2EIEvtcParser.EIData;

public class AgentFacingAgentConnectorDescription : AgentFacingConnectorDescription
{
    public readonly int DstMasterID;
    internal AgentFacingAgentConnectorDescription(AgentFacingAgentConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        var agent = connector.DstAgent;
        DstMasterID = agent.ParentAgentItem?.Merged.UniqueID ?? agent.UniqueID;
    }
}
