
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.MechanicIDs;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class MountBalrior : RaidWingLogic
{
    public MountBalrior(int triggerID) : base(triggerID)
    {
        MechanicList.AddRange(new List<Mechanic>
        {
            new PlayerDstBuffApplyMechanic(ExposedPlayer, Mech_ExposedPlayerWing8, new (Symbols.TriangleLeft, Colors.Purple, 10), new("Exposed", "Exposed Applied (Increased incoming damage)", "Exposed Applied"), Sev0),
            new PlayerDstBuffApplyMechanic(Debilitated, Mech_DebilitatedWing8, new (Symbols.TriangleDown, Colors.Purple, 10), new("Debilitated", "Debilitated Applied (Reduced outgoing damage)", "Debilitated Applied"), Sev0),
            new PlayerDstBuffApplyMechanic(Infirmity, Mech_InfirmityWing8, new (Symbols.TriangleUp, Colors.Purple, 10), new("Infirmity", "Infirmity Applied (Reduced incoming healing)", "Infirmity Applied"), Sev0),
            new EnemyDstBuffApplyMechanic(Exposed31589, Mech_ExposedWing8, new (Symbols.BowtieOpen, Colors.LightPurple), new("Brk.Brkn", "Got Exposed (Broke Breakbar)", "Exposed"), Sev1),
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
