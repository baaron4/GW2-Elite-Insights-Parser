
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic;

internal abstract class MountBalrior : RaidLogic
{
    public MountBalrior(int triggerID) : base(triggerID)
    {
        MechanicList.AddRange(new List<Mechanic>
        {
            new PlayerDstBuffApplyMechanic(ExposedPlayer, new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Purple, 10), "Exposed", "Exposed Applied (Increased incoming damage)", "Exposed Applied", 0),
            new PlayerDstBuffApplyMechanic(Debilitated, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Purple, 10), "Debilitated", "Debilitated Applied (Reduced outgoing damage)", "Debilitated Applied", 0),
            new PlayerDstBuffApplyMechanic(Infirmity, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Purple, 10), "Infirmity", "Infirmity Applied (Reduced incoming healing)", "Infirmity Applied", 0),
        });
        EncounterCategoryInformation.SubCategory = SubFightCategory.MountBalrior;
        EncounterID |= EncounterIDs.RaidWingMasks.MountBalriorMask;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadget(ChestID.GreersChest, agentData, combatData, GreersChestPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100));
        FindChestGadget(ChestID.DecimasChest, agentData, combatData, DecimasChestPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100));
        FindChestGadget(ChestID.UrasChest, agentData, combatData, UrasChestPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100));
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
    }
}
