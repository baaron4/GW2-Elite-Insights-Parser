using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIGW2API;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class NexusOfEternityConvergenceInstance : ConvergenceLogic
{
    public NexusOfEternityConvergenceInstance(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.NexusOfEternityConvergence;
        LogID |= LogIDs.ConvergenceMasks.NexusOfEternityConvergenceMask;
        Icon = InstanceIconNexusOfEternity;
        Extension = "noeconv";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        var name = "Convergence: Nexus of Eternity";
        return name;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations, CombatReplayMap? parentMap = null)
    {
        return base.GetCombatMapInternal(log, arenaDecorations, parentMap);
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
        ];
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        return
        [
        ];
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.Mode.Normal;
    }

    internal override LogData.InstancePrivacyMode GetInstancePrivacyMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.InstancePrivacyMode.Private;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        var phases = GetInitialPhase(log);
        return phases;
    }
}
