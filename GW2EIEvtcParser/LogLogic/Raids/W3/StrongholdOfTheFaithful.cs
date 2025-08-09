
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class StrongholdOfTheFaithful : RaidLogic
{
    public StrongholdOfTheFaithful(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.StrongholdOfTheFaithful;
        LogID |= LogIDs.RaidWingMasks.StrongholdOfTheFaithfulMask;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadget(ChestID.SiegeChest, agentData, combatData, SiegeChestPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100));
        FindChestGadget(ChestID.KeepConstructChest, agentData, combatData, KeepConstructChestPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100));
        FindChestGadget(ChestID.XeraChest, agentData, combatData, XeraChestPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100));
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }
}
