using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class StrongholdOfTheFaithful : RaidWingLogic
{
    public StrongholdOfTheFaithful(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.StrongholdOfTheFaithful;
        LogID |= LogIDs.RaidWingMasks.StrongholdOfTheFaithfulMask;
    }
    protected override (long downAndOutID, TargetID targetID) GetDownAndOutIDs()
    {
        return (DownAndOutStrongholdOfTheFaithful, TargetID.Xera);
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadgets([
            (ChestID.SiegeChest, SiegeChestPosition, 100),
            (ChestID.KeepConstructChest, KeepConstructChestPosition, 100),
            (ChestID.XeraChest, XeraChestPosition, 100),
        ], agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }
}
