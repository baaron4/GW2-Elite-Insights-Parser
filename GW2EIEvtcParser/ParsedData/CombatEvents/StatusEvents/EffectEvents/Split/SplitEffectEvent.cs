using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public abstract class SplitEffectEvent : EffectEvent
{
    protected static float OrientationAndScaleConvertConstant = 1f / 1000.0f;
    protected static float PositionConvertConstant = 10.0f;
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

    internal SplitEffectEvent(CombatItem evtcItem, AgentData agentData, IReadOnlyDictionary<long, EffectGUIDEvent> effectGUIDs) : base(evtcItem, agentData, effectGUIDs)
    {
        TrackingID = evtcItem.Pad;
        Duration = ReadDuration(evtcItem);
        if (Duration == 0 && GUIDEvent.DefaultDuration > 0)
        {
            Duration = (long)Math.Min(GUIDEvent.DefaultDuration, int.MaxValue); // To avoid overflow, end time could be start + duration, 13 days is more than enough to cover a log's duration
        }
    }
}
