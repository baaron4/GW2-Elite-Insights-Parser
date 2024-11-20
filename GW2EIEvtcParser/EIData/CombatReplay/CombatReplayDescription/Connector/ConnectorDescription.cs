using System.Text.Json.Serialization;

namespace GW2EIEvtcParser.EIData;

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(SkillConnectorDescription))]
[JsonDerivedType(typeof(RotationConnectorDescription))]
[JsonDerivedType(typeof(AngleConnectorDescription))]
[JsonDerivedType(typeof(AngleInterpolationConnectorDescription))]
[JsonDerivedType(typeof(AgentFacingAgentConnectorDescription))]
[JsonDerivedType(typeof(AgentFacingConnectorDescription))]
[JsonDerivedType(typeof(GeographicalConnectorDescription))]
[JsonDerivedType(typeof(PositionConnectorDescription))]
[JsonDerivedType(typeof(InterpolationConnectorDescription))]
[JsonDerivedType(typeof(AgentConnectorDescription))]
public abstract class ConnectorDescription
{
    protected ConnectorDescription(Connector connector, CombatReplayMap map, ParsedEvtcLog log)
    {
    }
}
