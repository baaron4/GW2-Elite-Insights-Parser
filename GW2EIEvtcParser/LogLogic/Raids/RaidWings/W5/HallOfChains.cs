using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class HallOfChains : RaidWingLogic
{
    public HallOfChains(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
                new PlayerDstBuffApplyMechanic(FracturedSpirit, new MechanicPlotlySetting(Symbols.Square,Colors.Green), "Orb CD", "Applied when taking green","Green port", 0),
                new PlayerDstBuffApplyMechanic(SourcePureOblivionBuff, new MechanicPlotlySetting(Symbols.HexagonOpen, Colors.Black), "10%", "Lifted by Pure Oblivion", "Pure Oblivion (10%)", 0),
            ]));
        LogCategoryInformation.SubCategory = SubLogCategory.HallOfChains;
        LogID |= LogIDs.RaidWingMasks.HallOfChainsMask;
    }
    protected override (long downAndOutID, TargetID targetID) GetDownAndOutIDs()
    {
        return (DownAndOutHallOfChains, TargetID.Dhuum);
    }
    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadgets([
            (ChestID.ChestOfDesmina, ChestOfDesminaPosition, 100),
            (ChestID.ChestOfSouls, ChestOfSoulsPosition, 100),
            (ChestID.GrenthChest, GrenthChestPosition, 100),
            (ChestID.DhuumChest, DhuumChestPosition, 100),
        ], agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }
}
