using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class AetherbladeHideoutInstance : EndOfDragonsStrike
{
    private readonly AetherbladeHideout _aetherbladeHideout;
    public AetherbladeHideoutInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
        Icon = EncounterIconAetherbladeHideout;
        Extension = "aetherhide_map";
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        _aetherbladeHideout = new AetherbladeHideout(NonIdentifiedSpecies);
        MechanicList.Add(_aetherbladeHideout.Mechanics);
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return _aetherbladeHideout.GetCombatReplayMap(log);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Strike Mission: Aetherblade Hideout";
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        fightData.SetSuccess(true, fightData.FightEnd);
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        // To use once GetPhases have been overriden
        //return _subLogic.GetTargetsIDs();
        return [
            TargetID.MaiTrinStrike,
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

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        AetherbladeHideout.FindFerrousBombsAndCleanMaiTrins(agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
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
