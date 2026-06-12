
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.LogLogic.LogCategories;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class MountBalrior : RaidWingLogic
{
    public MountBalrior(int triggerID) : base(triggerID)
    {
        MechanicList.AddRange(new List<Mechanic>
        {
            new PlayerDstBuffApplyMechanic(ExposedPlayer, new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Purple, 10), "Exposed", "Exposed Applied (Increased incoming damage)", "Exposed Applied", 0),
            new PlayerDstBuffApplyMechanic(Debilitated, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Purple, 10), "Debilitated", "Debilitated Applied (Reduced outgoing damage)", "Debilitated Applied", 0),
            new PlayerDstBuffApplyMechanic(Infirmity, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Purple, 10), "Infirmity", "Infirmity Applied (Reduced incoming healing)", "Infirmity Applied", 0),
            new EnemyDstBuffApplyMechanic(Exposed31589, new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.LightPurple), "Brk.Brkn", "Got Exposed (Broke Breakbar)", "Exposed", 0),
        });
        LogCategoryInformation.SubCategory = SubLogCategory.MountBalrior;
        LogID |= LogIDs.RaidWingMasks.MountBalriorMask;
    }
    protected override (long downAndOutID, TargetID targetID) GetDownAndOutIDs()
    {
        return (DownAndOutMountBalrior, TargetID.Ura);
    }
    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadgets([
            (ChestID.GreersChest, GreersChestPosition, 100),
            (ChestID.DecimasChest, DecimasChestPosition, 100),
            (ChestID.UrasChest, UrasChestPosition, 100),
        ], agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }
}
