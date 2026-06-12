using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class BastionOfThePenitent : RaidWingLogic
{
    public BastionOfThePenitent(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.BastionOfThePenitent;
        LogID |= LogIDs.RaidWingMasks.BastionOfThePenitentMask;
    }
    protected override (long downAndOutID, TargetID targetID) GetDownAndOutIDs()
    {
        return (DownAndOutBastionOfThePenitent, TargetID.Deimos);
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadgets([
            (ChestID.CairnChest, CairnChestPosition, 100),
            (ChestID.RecreationRoomChest, RecreationRoomChestPosition, 100),
            (ChestID.SamarogChest, SamarogChestPosition, 100),
            (ChestID.SaulsTreasureChest, SaulsTreasureChestPosition, 100),
        ], agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }
}
