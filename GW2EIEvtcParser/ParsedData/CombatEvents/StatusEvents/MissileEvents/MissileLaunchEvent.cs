using System.Numerics;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.Trigonometry;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData;

public class MissileLaunchEvent : TimeCombatEvent
{
    public bool LaunchedTowardsAgent => _targetedAgent != null && !_targetedAgent.IsNonIdentifiedSpecies();
    private readonly AgentItem? _targetedAgent = null;
    public AgentItem TargetedAgent => LaunchedTowardsAgent ? _targetedAgent! : _unknownAgent;

    public readonly Vector3 TargetPosition;
    public readonly Vector3 LaunchPosition;

    public readonly ushort Velocity;
    public MissileEvent Missile { get; internal set; }
    internal MissileLaunchEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem.Time)
    {
        if (evtcItem.DstAgent != 0)
        {
            _targetedAgent = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        } 
        TargetPosition = new(
            BitConverter.Int32BitsToSingle(evtcItem.Value),
            BitConverter.Int32BitsToSingle(evtcItem.BuffDmg),
            BitConverter.Int32BitsToSingle(unchecked((int)evtcItem.OverstackValue))
        );
        var velocityBytes = new ByteBuffer(stackalloc byte[1 * sizeof(ushort)]);
        // 0.5 
        velocityBytes.PushNative(evtcItem.IsShields);
        // 0.5
        velocityBytes.PushNative(evtcItem.IsOffcycle);
        unsafe
        {
            fixed (byte* ptr = velocityBytes.Span)
            {
                var velocityUShorts = (ushort*)ptr;
                Velocity = velocityUShorts[0];
            }
        }

        var launchPositionBytes = new ByteBuffer(stackalloc byte[3 * sizeof(float)]);
        // 0.5
        launchPositionBytes.PushNative(evtcItem.IFFByte);
        // 0.5
        launchPositionBytes.PushNative(evtcItem.IsBuff);
        // 0.5
        launchPositionBytes.PushNative(evtcItem.Result);
        // 0.5
        launchPositionBytes.PushNative(evtcItem.IsActivationByte);
        // 0.5
        launchPositionBytes.PushNative(evtcItem.IsBuffRemoveByte);
        // 0.5
        launchPositionBytes.PushNative(evtcItem.IsNinety);
        // 0.5
        launchPositionBytes.PushNative(evtcItem.IsFifty);
        // 0.5
        launchPositionBytes.PushNative(evtcItem.IsMoving);
        // 4
        launchPositionBytes.PushNative(evtcItem.Pad);
        unsafe
        {
            fixed (byte* ptr = launchPositionBytes.Span)
            {
                var launchPositionFloats = (float*)ptr;
                LaunchPosition = new(
                    launchPositionFloats[0],
                    launchPositionFloats[1],
                    launchPositionFloats[2]
                );
            }
        }
    }

}
