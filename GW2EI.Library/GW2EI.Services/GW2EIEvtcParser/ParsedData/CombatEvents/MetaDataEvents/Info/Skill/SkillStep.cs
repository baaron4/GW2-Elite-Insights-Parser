namespace GW2EIEvtcParser.ParsedData;

using static GW2EIEvtcParser.ArcDPSEnums;

public class SkillStep
{
    internal readonly SkillAnimation Animation;
    public readonly byte Type;
    public readonly int Index;
    public readonly uint Flags;
    public readonly int ActionDuration;
    public readonly int MoveDuration;

    public int Duration => Math.Max(ActionDuration, MoveDuration); // technically also distinguishable by type
    public bool IsLoopStart => (Flags & (uint)SkillStepFlags.LoopStart) > 0;
    public bool IsLoopEnd => (Flags & (uint)SkillStepFlags.LoopEnd) > 0;

    internal SkillStep(CombatItem evtcItem, SkillAnimation animation)
    {
        Animation = animation;
        Index = evtcItem.Value;

        var data = new byte[9];
        BitConverter.TryWriteBytes(data.AsSpan(), evtcItem.SrcAgent);
        data[8] = BitConverter.GetBytes(evtcItem.DstAgent)[7];

        Type = data[0];
        Flags = BitConverter.ToUInt32(data.AsSpan()[5..]);
        Index = evtcItem.Value;
        ActionDuration = evtcItem.BuffDmg;
        MoveDuration = BitConverter.ToInt32(BitConverter.GetBytes(evtcItem.OverstackValue));
    }

    public static bool IsUnderwater(CombatItem evtcItem)
    {
        return evtcItem.IFFByte != 0;
    }
}
