namespace GW2EIEvtcParser.ParsedData;

public class GadgetInfoEvent : AgentInfoEvent
{
    public readonly ArcDPSEnums.GadgetTypeEnum GadgetType = ArcDPSEnums.GadgetTypeEnum.NotApplicable;
    public readonly uint GadgetTypeValue = uint.MaxValue;

    internal GadgetInfoEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        GadgetTypeValue = (uint)evtcItem.Value;
        GadgetType = ArcDPSEnums.GetGadgetType(GadgetTypeValue, evtcItem.Build);
    }

}
