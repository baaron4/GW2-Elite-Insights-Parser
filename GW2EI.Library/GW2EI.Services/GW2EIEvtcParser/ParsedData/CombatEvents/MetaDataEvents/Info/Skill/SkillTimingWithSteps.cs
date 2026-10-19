namespace GW2EIEvtcParser.ParsedData;

public class SkillTimingWithSteps : SkillTiming
{
    public readonly ulong TriggerData;
    public byte[] TriggerDataBytes => BitConverter.GetBytes(TriggerData);

    public readonly int Index;

    public readonly bool Underwater;

    internal SkillTimingWithSteps(CombatItem evtcItem, SkillInfoEvent skillInfo) : base(evtcItem, skillInfo)
    {
        TriggerData = evtcItem.DstAgent;
        Index = evtcItem.Value;
        Underwater = evtcItem.IFFByte > 0;
    }

}
