using System.Collections.Generic;
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

internal class MythwrightGambitInstance : MythwrightGambit
{
    private readonly ConjuredAmalgamate _conjuredAmalgamate;
    private readonly TwinLargos _twinLargos;
    private readonly Qadim _qadim;

    private readonly IReadOnlyList<MythwrightGambit> _subLogics;
    public MythwrightGambitInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
        Icon = InstanceIconMythwrightGambit;
        Extension = "mythgamb";

        _conjuredAmalgamate = new ConjuredAmalgamate((int)TargetID.ConjuredAmalgamate);
        _twinLargos = new TwinLargos((int)TargetID.Nikare);
        _qadim = new Qadim((int)TargetID.Qadim);
        _subLogics = [_conjuredAmalgamate, _twinLargos, _qadim];

        MechanicList.Add(_conjuredAmalgamate.Mechanics);
        MechanicList.Add(_twinLargos.Mechanics);
        MechanicList.Add(_qadim.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Mythwright Gambit";
    }


    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        return base.GetPhases(log, requirePhases);
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _conjuredAmalgamate.GetInstantCastFinders(),
            .. _twinLargos.GetInstantCastFinders(),
            .. _qadim.GetInstantCastFinders()
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _conjuredAmalgamate.GetTrashMobsIDs(),
            .. _twinLargos.GetTrashMobsIDs(),
            .. _qadim.GetTrashMobsIDs()
        ];
        return trashes.Distinct().ToList();
    }
    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _conjuredAmalgamate.GetTargetsIDs(),
            .. _twinLargos.GetTargetsIDs(),
            .. _qadim.GetTargetsIDs()
        ];
        targets.Add(TargetID.Instance);
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _conjuredAmalgamate.GetFriendlyNPCIDs(),
            .. _twinLargos.GetFriendlyNPCIDs(),
            .. _qadim.GetFriendlyNPCIDs()
        ];
        return friendlies.Distinct().ToList();
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        ConjuredAmalgamate.HandleCAAgents(agentData, combatData);
        var sword = ConjuredAmalgamate.CreateCustomSwordAgent(fightData, agentData);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        ConjuredAmalgamate.RedirectSwordDamageToSwordAgent(sword, combatData, extensions);
    }

    internal override List<BuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
    {
        var res = new List<BuffEvent>();
        foreach (var subLogic in _subLogics)
        {
            res.AddRange(subLogic.SpecialBuffEventProcess(combatData, skillData));
        }
        return res;
    }

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
    {
        var res = new List<CastEvent>();
        foreach (var subLogic in _subLogics)
        {
            res.AddRange(subLogic.SpecialCastEventProcess(combatData, skillData));
        }
        return res;
    }

    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, SkillData skillData)
    {
        var res = new List<HealthDamageEvent>();
        foreach (var subLogic in _subLogics)
        {
            res.AddRange(subLogic.SpecialDamageEventProcess(combatData, skillData));
        }
        return res;
    }

    // TODO: handle duplicates due multiple base method calls in Combat Replay methods
    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputeNPCCombatReplayActors(target, log, replay);
        foreach (var logic in _subLogics)
        {
            logic.ComputeNPCCombatReplayActors(target, log, replay);
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        foreach (var logic in _subLogics)
        {
            logic.ComputePlayerCombatReplayActors(p, log, replay);
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        foreach (var logic in _subLogics)
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
}
