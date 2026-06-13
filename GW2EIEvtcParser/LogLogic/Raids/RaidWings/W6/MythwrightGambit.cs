using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class MythwrightGambit : RaidWingLogic
{
    public MythwrightGambit(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.MythwrightGambit;
        LogID |= LogIDs.RaidWingMasks.MythwrightGambitMask;
    }
    protected override (long downAndOutID, TargetID targetID) GetDownAndOutIDs()
    {
        return (DownAndOutMythwrightGambit, TargetID.Qadim);
    }
    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadgets([
            (ChestID.CAChest, CAChestPosition, 100),
            (ChestID.TwinLargosChest, TwinLargosChestPosition, 100),
            (ChestID.QadimsChest, QadimsChestPosition, 100),
        ], agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }
}
