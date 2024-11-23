using System.Numerics;

namespace GW2EIEvtcParser.EIData;

internal abstract class GeographicalConnector : Connector
{
    public Vector3? Offset { get; private set; }

    public bool OffsetAfterRotation { get; private set; }
    public bool InvertYOffset { get; private set; } = false;

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
        InvertYOffset = invertY;
        return this;
    }
}
