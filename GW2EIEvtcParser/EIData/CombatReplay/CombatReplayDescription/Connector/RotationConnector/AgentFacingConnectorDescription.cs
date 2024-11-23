namespace GW2EIEvtcParser.EIData;

public class AgentFacingConnectorDescription : RotationConnectorDescription
{
    public int MasterId { get; private set; }
    public float RotationOffset { get; private set; }
    public int RotationOffsetMode { get; private set; }
    internal AgentFacingConnectorDescription(AgentFacingConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        MasterId = connector.Agent.UniqueID;
        RotationOffset = connector.RotationOffset;
        RotationOffsetMode = (int)connector.OffsetMode;
    }
}
