using System.Numerics;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData;

public class EffectEventCBTS51 : EffectEvent
{
    const float OrientationConvertConstant = 1f / 1000.0f;

    //TODO(Rennorb) @cleanup: replace with union
    internal static Vector3 ReadOrientation(CombatItem evtcItem)
    {
        var orientationBytes = new byte[3 * sizeof(short)];
        int offset = 0;
        orientationBytes[offset++] = evtcItem.IsShields;
        orientationBytes[offset++] = evtcItem.IsOffcycle;
        orientationBytes[offset++] = evtcItem.Pad1;
        orientationBytes[offset++] = evtcItem.Pad2;
        orientationBytes[offset++] = evtcItem.Pad3;
        orientationBytes[offset++] = evtcItem.Pad4;


        var orientationInt = new short[3];
        Buffer.BlockCopy(orientationBytes, 0, orientationInt, 0, orientationBytes.Length);

        return new Vector3(orientationInt[0], orientationInt[1], -orientationInt[2]) * OrientationConvertConstant;
    }

    private static uint ReadDuration(CombatItem evtcItem)
    {
        var durationBytes = new byte[sizeof(uint)];
        int offset = 0;
        durationBytes[offset++] = evtcItem.IFFByte;
        durationBytes[offset++] = evtcItem.IsBuff;
        durationBytes[offset++] = evtcItem.Result;
        durationBytes[offset++] = evtcItem.IsActivationByte;


        var durationUInt = new uint[1];
        Buffer.BlockCopy(durationBytes, 0, durationUInt, 0, durationBytes.Length);
        return durationUInt[0];
    }

    internal static uint ReadTrackingID(CombatItem evtcItem)
    {
        var trackingIDBytes = new byte[sizeof(uint)];
        int offset = 0;
        trackingIDBytes[offset++] = evtcItem.IsBuffRemoveByte;
        trackingIDBytes[offset++] = evtcItem.IsNinety;
        trackingIDBytes[offset++] = evtcItem.IsFifty;
        trackingIDBytes[offset++] = evtcItem.IsMoving;


        var trackingIDUInt = new uint[1];
        Buffer.BlockCopy(trackingIDBytes, 0, trackingIDUInt, 0, trackingIDBytes.Length);
        return trackingIDUInt[0];
    }

    internal EffectEventCBTS51(CombatItem evtcItem, AgentData agentData, IReadOnlyDictionary<long, EffectGUIDEvent> effectGUIDs, Dictionary<long, List<EffectEvent>> effectEventsByTrackingID) : base(evtcItem, agentData, effectGUIDs)
    {
        Orientation = ReadOrientation(evtcItem);
        Duration = ReadDuration(evtcItem);
        if (Duration == 0 && GUIDEvent.DefaultDuration > 0)
        {
            Duration = (long)Math.Min(GUIDEvent.DefaultDuration, int.MaxValue); // To avoid overflow, end time could be start + duration, 13 days is more than enough to cover a log's duration
        }
        TrackingID = ReadTrackingID(evtcItem);
        OnNonStaticPlatform = evtcItem.IsFlanking > 0;
        if (TrackingID != 0)
        {
            Add(effectEventsByTrackingID, TrackingID, this);
        }
    }

}
