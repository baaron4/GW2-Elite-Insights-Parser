namespace GW2EIEvtcParser.ParsedData;

public class AgentInfoEvent : StatusEvent
{
    public ParserHelper.Spec Type => Src.Spec;

    public readonly ArcDPSEnums.GadgetTypeEnum GadgetType = ArcDPSEnums.GadgetTypeEnum.NotApplicable;
    public readonly uint GadgetTypeValue = uint.MaxValue;
    public readonly uint SpeciesFlags = 0;

    public bool IsVeteran => (SpeciesFlags & (uint)ArcDPSEnums.SpeciesFlags.Veteran) > 0;
    public bool IsChampion => (SpeciesFlags & (uint)ArcDPSEnums.SpeciesFlags.Champion) > 0;
    public bool IsElite => (SpeciesFlags & (uint)ArcDPSEnums.SpeciesFlags.Elite) > 0;
    public bool IsLegendary => (SpeciesFlags & (uint)ArcDPSEnums.SpeciesFlags.Legendary) > 0;

    internal AgentInfoEvent(CombatItem evtcItem, AgentData agentData, EvtcVersionEvent evtcVersion) : base(evtcItem, agentData)
    {
        // TODO Type
        if (Type == ParserHelper.Spec.Gadget)
        {
            GadgetTypeValue = (uint)evtcItem.Value;
            GadgetType = ArcDPSEnums.GetGadgetType(GadgetTypeValue, evtcVersion);
        }
        else if (Type == ParserHelper.Spec.NPC)
        {
            SpeciesFlags = (uint)evtcItem.BuffDmg;
        }
    }

}
