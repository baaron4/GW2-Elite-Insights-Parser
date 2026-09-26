namespace GW2EIEvtcParser.ParsedData;

public class AgentInfoEvent : StatusEvent
{
    public readonly AgentItem.AgentType Type;

    public readonly ArcDPSEnums.GadgetTypeEnum GadgetType = ArcDPSEnums.GadgetTypeEnum.NotApplicable;
    public readonly uint GadgetTypeValue = uint.MaxValue;

    public readonly ArcDPSEnums.SpeciesFlagsEnum SpeciesFlags = ArcDPSEnums.SpeciesFlagsEnum.NotApplicable;
    public readonly uint SpeciesFlagsValue = uint.MaxValue;

    internal AgentInfoEvent(CombatItem evtcItem, AgentData agentData, EvtcVersionEvent evtcVersion) : base(evtcItem, agentData)
    {
        // TODO Type
        if (Type == AgentItem.AgentType.VolatileSpecies)
        {
            GadgetTypeValue = (uint)evtcItem.Value;
            GadgetType = ArcDPSEnums.GetGadgetType(GadgetTypeValue, evtcVersion);
        } 
        else if (Type == AgentItem.AgentType.StableSpecies)
        {
            SpeciesFlagsValue = (uint)evtcItem.BuffDmg;
            SpeciesFlags = ArcDPSEnums.GetSpeciesFlags(SpeciesFlagsValue, evtcVersion);
        }
    }

}
