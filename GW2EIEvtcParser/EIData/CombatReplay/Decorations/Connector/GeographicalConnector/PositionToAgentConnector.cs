using System.Numerics;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class PositionToAgentConnector : AgentConnector
{
    public readonly Vector3 Position;
    public readonly long StartTime;
    public readonly float Velocity;

    public PositionToAgentConnector(SingleActor agent, Vector3 position, long startTime, float velocity) : this(agent.AgentItem, position, startTime, velocity)
    {
    }

    public PositionToAgentConnector(AgentItem agent, Vector3 position, long startTime, float velocity) : base(agent)
    {
        Position = position;
        StartTime = startTime;
        Velocity = velocity;
    }

    public override ConnectorDescription GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new PositionToAgentConnectorDescription(this, map, log);
    }
}
