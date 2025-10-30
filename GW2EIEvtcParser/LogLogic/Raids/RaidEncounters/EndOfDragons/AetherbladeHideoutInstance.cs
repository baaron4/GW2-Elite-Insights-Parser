using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SpeciesIDs;
using GW2EIGW2API;

namespace GW2EIEvtcParser.LogLogic;

internal class AetherbladeHideoutInstance : EndOfDragonsRaidEncounter
{
    private readonly AetherbladeHideout _aetherbladeHideout;
    public AetherbladeHideoutInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
        Icon = EncounterIconAetherbladeHideout;
        Extension = "aetherhide_map";
        LogCategoryInformation.InSubCategoryOrder = 0;
        _aetherbladeHideout = new AetherbladeHideout(NonIdentifiedSpecies);
        MechanicList.Add(_aetherbladeHideout.Mechanics);
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        return _aetherbladeHideout.GetCombatReplayMap(log);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Strike Mission: Aetherblade Hideout";
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        logData.SetSuccess(true, logData.LogEnd);
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        // To use once GetPhases have been overriden
        //return _subLogic.GetTargetsIDs();
        return [
            TargetID.MaiTrinRaid,
            TargetID.EchoOfScarletBriarNM,
            TargetID.EchoOfScarletBriarCM,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return _aetherbladeHideout.GetTrashMobsIDs();
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return _aetherbladeHideout.GetInstantCastFinders();
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        AetherbladeHideout.FindFerrousBombsAndCleanMaiTrins(agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        AetherbladeHideout.SanitizeLastHealthUpdateEvents(Targets, combatData);
        AetherbladeHideout.RenameScarletPhantoms(Targets);
    }
    internal override List<BuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
    {
        return _aetherbladeHideout.SpecialBuffEventProcess(combatData, skillData);
    }

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
    {
        return _aetherbladeHideout.SpecialCastEventProcess(combatData, skillData);
    }

    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        return _aetherbladeHideout.SpecialDamageEventProcess(combatData, agentData, skillData);
    }

    // TODO: handle duplicates due multiple base method calls in Combat Replay methods
    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        _aetherbladeHideout.ComputeNPCCombatReplayActors(target, log, replay);
    }
    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        _aetherbladeHideout.ComputePlayerCombatReplayActors(player, log, replay);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        _aetherbladeHideout.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
    }
    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        var sortIDs = new Dictionary<TargetID, int>();
        AddSortIDWithOffset(sortIDs, _aetherbladeHideout.GetTargetsSortIDs(), 0);
        return sortIDs;
    }
}
