namespace GW2EIEvtcParser.ParsedData;

public class SkillTiming
{

    public readonly byte ActionByte;

    public readonly ArcDPSEnums.SkillAction Action;

    public readonly ulong AtMillisecond;

    internal SkillTiming(CombatItem evtcItem)
    {
        ActionByte = (byte)evtcItem.SrcAgent;
        Action = ArcDPSEnums.GetSkillAction(ActionByte);
        AtMillisecond = evtcItem.DstAgent;
    }

}
