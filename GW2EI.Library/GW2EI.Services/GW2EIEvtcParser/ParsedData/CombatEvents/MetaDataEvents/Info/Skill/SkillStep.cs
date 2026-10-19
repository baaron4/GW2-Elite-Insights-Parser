using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public class SkillStep
{
    internal readonly SkillInfoEvent SkillInfo;
    public readonly int Index;

    public readonly byte[] ClientData;
    public readonly byte[] Ptr0;
    public readonly byte[] Ptr1;

    internal SkillStep(CombatItem evtcItem, SkillInfoEvent skillInfo)
    {
        SkillInfo = skillInfo;
        Index = evtcItem.Value;

        ClientData = new byte[9];
        var clientData1Span = ClientData.AsSpan();
        BitConverter.TryWriteBytes(clientData1Span, evtcItem.SrcAgent);
        ClientData[8] = BitConverter.GetBytes(evtcItem.DstAgent)[7];

        Ptr0 = evtcItem.BuffDmg != -1 ? BitConverter.GetBytes(evtcItem.BuffDmg) : [];

        Ptr1 = (int)evtcItem.OverstackValue != -1 ? BitConverter.GetBytes(evtcItem.OverstackValue) : [];
    }

}
