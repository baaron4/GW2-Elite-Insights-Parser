using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class AetherbladeHideoutInstance : EndOfDragonsStrike
{
    private readonly AetherbladeHideout _subLogic;
    public AetherbladeHideoutInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
        Icon = EncounterIconAetherbladeHideout;
        Extension = "aetherhide_map";
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        _subLogic = new AetherbladeHideout(NonIdentifiedSpecies);
        MechanicList.Add(_subLogic.Mechanics);
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return _subLogic.GetCombatReplayMap(log);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Aetherblade Hideout (Map)";
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        AetherbladeHideout.PreHandleAgents(agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        AetherbladeHideout.PostProcessEvtcEvents(Targets, combatData);
        AetherbladeHideout.HandleCustomRenaming(Targets);
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
        return _subLogic.GetTrashMobsIDs();
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return _subLogic.GetInstantCastFinders();
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        _subLogic.ComputePlayerCombatReplayActors(player, log, replay);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
    {
        _subLogic.ComputeEnvironmentCombatReplayDecorations(log);
    }
}
