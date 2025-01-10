using System.Numerics;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData;

public abstract class AbstractEffectEvent : StatusEvent
{

    /// <summary>
    /// The effect's rotation around each axis in <b>radians</b>.
    /// Use <see cref="Rotation"/> for degrees.
    /// </summary>
    public Vector3 Orientation { get; protected set; }

    /// <summary>
    /// The effect's rotation around each axis in <b>degrees</b>.
    /// Like <see cref="Orientation"/> but using degrees.
    /// </summary>
    public Vector3 Rotation => new(RadianToDegreeF(Orientation.X), RadianToDegreeF(Orientation.Y), RadianToDegreeF(Orientation.Z));

    /// <summary>
    /// The effect's position in the game's coordinate system, if <see cref="IsAroundDst"/> is <c>false</c>.
    /// </summary>
    public readonly Vector3 Position = new(0, 0, 0);

    /// <summary>
    /// Whether the effect location is following <see cref="Dst"/> or located at <see cref="Position"/>.
    /// </summary>
    public bool IsAroundDst => _dst != null;
    /// <summary>
    /// The agent the effect is located at, if <see cref="IsAroundDst"/> is <c>true</c>.
    /// </summary>
    private readonly AgentItem? _dst = null;
    /// <summary>
    /// The agent the effect is located at, if <see cref="IsAroundDst"/> is <c>true</c>.
    /// </summary>
    public AgentItem Dst => IsAroundDst ? _dst! : _unknownAgent;

    /// <summary>
    /// Unique id for tracking a created effect.
    /// </summary>
    protected long TrackingID;

    internal AbstractEffectEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        if (evtcItem.DstAgent != 0)
        {
            _dst = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        }
        else
        {
            Position = new(
                BitConverter.Int32BitsToSingle(evtcItem.Value),
                BitConverter.Int32BitsToSingle(evtcItem.BuffDmg),
                BitConverter.Int32BitsToSingle(unchecked((int)evtcItem.OverstackValue))
            );
        }
    }
}
