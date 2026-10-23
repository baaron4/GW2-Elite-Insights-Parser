namespace GW2EIEvtcParser.ParsedData;

public class SkillTimingWithSteps : SkillTiming
{
    public readonly int Index;
    public readonly uint Flags;

    internal SkillTimingWithSteps(CombatItem evtcItem, SkillAnimation animation) : base(evtcItem, animation)
    {
        var data = BitConverter.GetBytes(evtcItem.DstAgent);
        Time = BitConverter.ToInt32(data.AsSpan()[..4]);
        Flags = BitConverter.ToUInt32(data.AsSpan()[4..]);
        Index = evtcItem.Value;
    }

    public static new bool IsUnderwater(CombatItem evtcItem)
    {
        return evtcItem.IFFByte != 0;
    }
}
