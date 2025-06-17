using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class StrongholdOfTheFaithfulInstance : StrongholdOfTheFaithful
{
    private readonly Escort _escort;
    private readonly KeepConstruct _keepConstruct;
    private readonly TwistedCastle _twistedCastle;
    private readonly Xera _xera;

    private readonly IReadOnlyList<StrongholdOfTheFaithful> _subLogics;
    public StrongholdOfTheFaithfulInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
        Icon = InstanceIconStrongholdOfTheFaithful;
        Extension = "strgldfaith";

        _escort = new Escort((int)TargetID.McLeodTheSilent);
        _keepConstruct = new KeepConstruct((int)TargetID.KeepConstruct);
        _twistedCastle = new TwistedCastle((int)TargetID.DummyTarget);
        _xera = new Xera((int)TargetID.Xera);
        _subLogics = [_escort, _keepConstruct, _twistedCastle, _xera];

        MechanicList.Add(_escort.Mechanics);
        MechanicList.Add(_keepConstruct.Mechanics);
        MechanicList.Add(_twistedCastle.Mechanics);
        MechanicList.Add(_xera.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Stronghold Of The Faithful";
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _escort.GetInstantCastFinders(),
            .. _keepConstruct.GetInstantCastFinders(),
            .. _twistedCastle.GetInstantCastFinders(),
            .. _xera.GetInstantCastFinders()
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _escort.GetTrashMobsIDs(),
            .. _keepConstruct.GetTrashMobsIDs(),
            .. _twistedCastle.GetTrashMobsIDs(),
            .. _xera.GetTrashMobsIDs()
        ];
        return trashes.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _escort.GetTargetsIDs(),
            .. _keepConstruct.GetTargetsIDs(),
            .. _twistedCastle.GetTargetsIDs(),
            .. _xera.GetTargetsIDs()
        ];
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _escort.GetFriendlyNPCIDs(),
            .. _keepConstruct.GetFriendlyNPCIDs(),
            .. _twistedCastle.GetFriendlyNPCIDs(),
            .. _xera.GetFriendlyNPCIDs()
        ];
        return friendlies.Distinct().ToList();
    }

    // TODO: handle duplicates due multiple base method calls in Combat Replay methods
    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputeNPCCombatReplayActors(target, log, replay);
        foreach (StrongholdOfTheFaithful logic in _subLogics)
        {
            logic.ComputeNPCCombatReplayActors(target, log, replay);
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        foreach (StrongholdOfTheFaithful logic in _subLogics)
        {
            logic.ComputePlayerCombatReplayActors(p, log, replay);
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        Escort.FindMines(agentData, combatData);
        // For encounters before reaching McLeod
        agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Escort", Spec.NPC, TargetID.DummyTarget, true);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        Escort.RenameSubMcLeods(Targets);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        foreach (StrongholdOfTheFaithful logic in _subLogics)
        {
            logic.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        var sortIDs = new Dictionary<TargetID, int>();
        int offset = 0;
        foreach (var logic in _subLogics)
        {
            offset = AddSortIDWithOffset(sortIDs, logic.GetTargetsSortIDs(), offset);
        }
        return sortIDs;
    }
    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.EncounterMode.NotApplicable;
    }
}
