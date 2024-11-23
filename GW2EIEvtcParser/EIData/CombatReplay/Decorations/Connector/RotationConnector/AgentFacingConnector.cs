using System.Numerics;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class AgentFacingConnector : RotationConnector
{
    public readonly AgentItem Agent;

    /// <summary>
    /// Offset angle around Z axis, in degrees
    /// </summary>
    public readonly float RotationOffset = 0;

    public enum RotationOffsetMode
    {
        AddToMaster = 0,
        AbsoluteOrientation = 1,
        RotateAfterTranslationOffset = 2,
    }

    public readonly RotationOffsetMode OffsetMode = RotationOffsetMode.RotateAfterTranslationOffset;

    public AgentFacingConnector(SingleActor agent) : this(agent.AgentItem)
    {
    }

    public AgentFacingConnector(AgentItem agent)
    {
        Agent = agent;
    }

    public AgentFacingConnector(SingleActor agent, float rotationOffset, RotationOffsetMode rotationOffsetMode) : this(agent.AgentItem, rotationOffset, rotationOffsetMode)
    {

    }

    public AgentFacingConnector(AgentItem agent, float rotationOffset, RotationOffsetMode rotationOffsetMode) : this(agent)
    {
        RotationOffset = rotationOffset;
        OffsetMode = rotationOffsetMode;
    }

    public AgentFacingConnector(SingleActor agent, in Vector3 facingDirection, RotationOffsetMode rotationOffsetMode) : this(agent, facingDirection.GetRoundedZRotationDeg(), rotationOffsetMode)
    {
    }

    public AgentFacingConnector(AgentItem agent, in Vector3 facingDirection, RotationOffsetMode rotationOffsetMode) : this(agent, facingDirection.GetRoundedZRotationDeg(), rotationOffsetMode)
    {
    }

    public override ConnectorDescription GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new AgentFacingConnectorDescription(this, map, log);
    }
}
