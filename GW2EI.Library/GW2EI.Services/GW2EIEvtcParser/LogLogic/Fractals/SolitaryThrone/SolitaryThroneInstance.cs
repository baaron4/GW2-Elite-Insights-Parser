using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIGW2API;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class SolitaryThroneInstance : SolitaryThrone
{
    private readonly EternalTyrant _eternalTyrant;

    public SolitaryThroneInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
        Icon = "";
        Extension = "throne";

        _eternalTyrant = new EternalTyrant((int)TargetID.EternalTyrant);

        MechanicList.Add(_eternalTyrant.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Solitary Throne Fractal";
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _eternalTyrant.GetInstantCastFinders(),
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _eternalTyrant.GetTrashMobsIDs(),
        ];
        return trashes.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _eternalTyrant.GetTargetsIDs(),
        ];
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _eternalTyrant.GetFriendlyNPCIDs(),
        ];
        return friendlies.Distinct().ToList();
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override List<BuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
    {
        return _eternalTyrant.SpecialBuffEventProcess(combatData, skillData);
    }

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, AgentData agentData, SkillData skillData, Dictionary<long, List<AnimatedCastEvent>> animatedCastDataByID)
    {
        return _eternalTyrant.SpecialCastEventProcess(combatData, agentData, skillData, animatedCastDataByID);
    }

    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        return _eternalTyrant.SpecialDamageEventProcess(combatData, agentData, skillData);
    }
    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputeNPCCombatReplayActors(target, log, replay);
        _eternalTyrant.ComputeNPCCombatReplayActors(target, log, replay);
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        _eternalTyrant.ComputePlayerCombatReplayActors(p, log, replay);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        _eternalTyrant.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return _eternalTyrant.GetTargetsSortIDs();
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        base.SetInstanceBuffs(log, instanceBuffs);
        _eternalTyrant.SetInstanceBuffs(log, instanceBuffs);
    }

    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        _eternalTyrant.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
    }
}
