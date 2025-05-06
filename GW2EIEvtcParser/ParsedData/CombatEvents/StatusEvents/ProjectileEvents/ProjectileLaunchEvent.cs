using System.Numerics;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.Trigonometry;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData;

public class ProjectileLaunchEvent : TimeCombatEvent
{
    public bool LaunchedTowardsAgent => _targetedAgent != null;
    private readonly AgentItem? _targetedAgent = null;
    public AgentItem TargetedAgent => LaunchedTowardsAgent ? _targetedAgent! : _unknownAgent;

    public readonly Vector3 TargetPosition = new(0, 0, 0);

    public readonly float Velocity;
    public ProjectileEvent Projectile { get; internal set; }
    internal ProjectileLaunchEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem.Time)
    {
        if (evtcItem.DstAgent != 0)
        {
            _targetedAgent = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        } 
        else
        {
            TargetPosition = new(
                BitConverter.Int32BitsToSingle(evtcItem.Value),
                BitConverter.Int32BitsToSingle(evtcItem.BuffDmg),
                BitConverter.Int32BitsToSingle(unchecked((int)evtcItem.OverstackValue))
            );
        }
        var velocityBytes = new ByteBuffer(stackalloc byte[1 * sizeof(float)]);
        // 0.5 
        velocityBytes.PushNative(evtcItem.IFFByte);
        // 0.5
        velocityBytes.PushNative(evtcItem.IsBuff);
        // 0.5 
        velocityBytes.PushNative(evtcItem.Result);
        // 0.5
        velocityBytes.PushNative(evtcItem.IsActivationByte);
        unsafe
        {
            fixed (byte* ptr = velocityBytes.Span)
            {
                var velocityFloats = (float*)ptr;
                Velocity = velocityFloats[0];
            }
        }
    }

}
