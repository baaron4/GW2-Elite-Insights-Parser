using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.LogLogic.LogCategories;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class TheKeyOfAhdashim : RaidWingLogic
{
    public TheKeyOfAhdashim(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.TheKeyOfAhdashim;
        LogID |= LogIDs.RaidWingMasks.TheKeyOfAhdashimMask;
    }
    protected override (long downAndOutID, TargetID targetID) GetDownAndOutIDs()
    {
        return (DownAndOutKeyOfAhdashim, TargetID.QadimThePeerless);
    }
    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadgets([
            (ChestID.AdinasChest, AdinasChestPosition, 100),
            (ChestID.SabirsChest, SabirsChestPosition, 100),
            (ChestID.QadimThePeerlessChest, QadimThePeerlessChestPosition, 100),
        ], agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }
}
