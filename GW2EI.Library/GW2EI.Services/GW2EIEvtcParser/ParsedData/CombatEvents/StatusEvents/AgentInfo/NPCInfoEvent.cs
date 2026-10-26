namespace GW2EIEvtcParser.ParsedData;

public class NPCInfoEvent : AgentInfoEvent
{
    public readonly uint SpeciesFlags = 0;

    public IReadOnlyList<int> SpeciesFlagsAsBits => ParserHelper.GetIndividualFlagsAsBits(SpeciesFlags);

    public bool IsVeteran => (SpeciesFlags & (uint)ArcDPSEnums.SpeciesFlags.Veteran) > 0;
    public bool IsChampion => (SpeciesFlags & (uint)ArcDPSEnums.SpeciesFlags.Champion) > 0;
    public bool IsElite => (SpeciesFlags & (uint)ArcDPSEnums.SpeciesFlags.Elite) > 0;
    public bool IsLegendary => (SpeciesFlags & (uint)ArcDPSEnums.SpeciesFlags.Legendary) > 0;
    public bool DecorateName => (SpeciesFlags & (uint)ArcDPSEnums.SpeciesFlags.NoDecorateNameTBC) == 0;

    internal NPCInfoEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        SpeciesFlags = (uint)evtcItem.BuffDmg;
    }

}
