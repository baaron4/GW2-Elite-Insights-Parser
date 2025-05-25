using System.Numerics;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData;

public class EffectEventGroundCreate : SplitEffectEvent
{
    internal EffectEventGroundCreate(CombatItem evtcItem, AgentData agentData, IReadOnlyDictionary<long, EffectGUIDEvent> effectGUIDs, Dictionary<long, List<EffectEventGroundCreate>> effectEventsByTrackingID) : base(evtcItem, agentData, effectGUIDs)
    {
        // Vectors
        var vectorBytes = new ByteBuffer(stackalloc byte[6 * sizeof(short)]);
        // 2 
        vectorBytes.PushNative(evtcItem.DstAgent);
        // 1
        vectorBytes.PushNative(evtcItem.Value);
        unsafe
        {
            fixed (byte* ptr = vectorBytes.Span)
            {
                var vectorShorts = (short*)ptr;
                Position = new(
                        vectorShorts[0] * PositionConvertConstant,
                        vectorShorts[1] * PositionConvertConstant,
                        vectorShorts[2] * PositionConvertConstant
                    );
                Orientation = new(
                        vectorShorts[3] * OrientationAndScaleConvertConstant,
                        vectorShorts[4] * OrientationAndScaleConvertConstant,
                        vectorShorts[5] * OrientationAndScaleConvertConstant
                    );
            }
        }
        // Scale
        var scaleBytes = new ByteBuffer(stackalloc byte[sizeof(ushort)]);
        scaleBytes.PushNative(evtcItem.IsShields);
        scaleBytes.PushNative(evtcItem.IsOffcycle);
        unsafe
        {
            fixed (byte* ptr = scaleBytes.Span)
            {
                var scaleShorts = (short*)ptr;
                Scale = scaleShorts[0] * OrientationAndScaleConvertConstant;
            }
        }
        // ScaleSomething
        var scaleSomethingBytes = new ByteBuffer(stackalloc byte[sizeof(ushort)]);
        scaleSomethingBytes.PushNative(evtcItem.IsFifty);
        scaleSomethingBytes.PushNative(evtcItem.IsMoving);
        unsafe
        {
            fixed (byte* ptr = scaleSomethingBytes.Span)
            {
                var scaleSomethingShorts = (short*)ptr;
                ScaleSomething = scaleSomethingShorts[0] * OrientationAndScaleConvertConstant;
            }
        }
        //
        Flags = evtcItem.IsBuffRemoveByte;
        OnNonStaticPlatform = evtcItem.IsFlanking > 0;
        if (TrackingID != 0)
        {
            Add(effectEventsByTrackingID, TrackingID, this);
        }
    }

}
