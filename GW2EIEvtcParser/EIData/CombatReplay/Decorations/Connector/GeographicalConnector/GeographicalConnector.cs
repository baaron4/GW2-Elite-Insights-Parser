using System.Numerics;

namespace GW2EIEvtcParser.EIData;

public abstract class GeographicalConnector : Connector
{
    private Vector3? Offset;

    private bool OffsetAfterRotation;
    private bool _invertYOffset = false;
    public abstract class GeographicalConnectorDescriptor
    {
        public readonly IReadOnlyList<float>? Offset;
        public readonly bool OffsetAfterRotation;

        public GeographicalConnectorDescriptor(GeographicalConnector connector, CombatReplayMap map)
        {
            if (connector.Offset.HasValue)
            {
                OffsetAfterRotation = connector.OffsetAfterRotation;
                Offset = [
                    connector.Offset.Value.X,
                    connector._invertYOffset ? -connector.Offset.Value.Y : connector.Offset.Value.Y,
                ];
            }
        }
    }

    /// <summary>
    /// Adds an offset by the specified amount in the orientation given in <b>radians</b>. 
    /// </summary>
    public GeographicalConnector WithOffset(float orientation, float amount, bool afterRotation)
    {
        orientation *= -1; // game is indirect
        Offset = amount * new Vector3((float)Math.Cos(orientation), (float)Math.Sin(orientation), 0);
        OffsetAfterRotation = afterRotation;
        return this;
    }

    /// <summary>
    /// Adds an offset by the specified Point3D. 
    /// </summary>
    public GeographicalConnector WithOffset(Vector3 offset, bool afterRotation, bool invertY = false) //TODO(Rennorb) @cleanup: should this jsut be vec2 ?
    {
        Offset = offset;
        OffsetAfterRotation = afterRotation;
        _invertYOffset = invertY;
        return this;
    }
}
