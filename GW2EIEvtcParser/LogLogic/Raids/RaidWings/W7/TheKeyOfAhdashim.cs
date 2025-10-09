using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
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

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadgets([
            (ChestID.AdinasChest, AdinasChestPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100)),
            (ChestID.SabirsChest, SabirsChestPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100)),
            (ChestID.QadimThePeerlessChest, QadimThePeerlessChestPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100)),
        ], agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }
}
