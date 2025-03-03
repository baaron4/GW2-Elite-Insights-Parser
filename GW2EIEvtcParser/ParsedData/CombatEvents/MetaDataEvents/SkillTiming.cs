namespace GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

public class SkillTiming
{

    public readonly byte ActionByte;

    public readonly SkillAction Action;

    public readonly ulong AtMillisecond;

    internal SkillTiming(CombatItem evtcItem)
    {
        ActionByte = (byte)evtcItem.SrcAgent;
        Action = GetSkillAction(ActionByte);
        AtMillisecond = evtcItem.DstAgent;
    }

}
