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

internal class SalvationPassInstance : SalvationPass
{
    private readonly Slothasor _slothasor;
    private readonly BanditTrio _banditTrio;
    private readonly Matthias _matthias;

    private readonly IReadOnlyList<SalvationPass> _subLogics;
    public SalvationPassInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
        Icon = InstanceIconSalvationPass;
        Extension = "salvpass";

        _slothasor = new Slothasor((int)TargetID.Slothasor);
        _banditTrio = new BanditTrio((int)TargetID.Narella);
        _matthias = new Matthias((int)TargetID.Matthias);
        _subLogics = [_slothasor, _banditTrio, _matthias];

        MechanicList.Add(_slothasor.Mechanics);
        MechanicList.Add(_banditTrio.Mechanics);
        MechanicList.Add(_matthias.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Salvation Pass";
    }

    internal static void HandleTrioPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var packedTrios = new List<List<SingleActor>>();
        if (targetsByIDs.TryGetValue((int)TargetID.Narella, out var narellas))
        {
            if (targetsByIDs.TryGetValue((int)TargetID.Zane, out var zanes))
            {
                if (targetsByIDs.TryGetValue((int)TargetID.Berg, out var berg))
                {
                    foreach (var narella in narellas)
                    {
                        var pack = new List<SingleActor>();
                        pack.Add(narella);
                        var curZane = zanes.FirstOrDefault(x => x.AgentItem.InAwareTimes(narella.AgentItem));
                        if (curZane != null)
                        {
                            pack.Add(curZane);
                        }
                        var curBerg = zanes.FirstOrDefault(x => x.AgentItem.InAwareTimes(narella.AgentItem));
                        if (curBerg != null)
                        {
                            pack.Add(curBerg);
                        }
                        if (pack.Count == 3 && pack.Any(x => log.CombatData.GetEnterCombatEvents(x.AgentItem).Count > 0))
                        {
                            packedTrios.Add(pack);
                        }
                    }
                }
            }
        }
        // Thrash mob start check
        var boxStart = new Vector2(-2200, -11300);
        var boxEnd = new Vector2(1000, -7200);
        TargetID[] trashMobsToCheck =
        {
                TargetID.BanditAssassin,
                TargetID.BanditAssassin2,
                TargetID.BanditSapperTrio,
                TargetID.BanditDeathsayer,
                TargetID.BanditDeathsayer2,
                TargetID.BanditBrawler,
                TargetID.BanditBrawler2,
                TargetID.BanditBattlemage,
                TargetID.BanditBattlemage2,
                TargetID.BanditCleric,
                TargetID.BanditCleric2,
                TargetID.BanditBombardier,
                TargetID.BanditSniper,
        };
        var bandits = log.AgentData.GetNPCsByIDs(trashMobsToCheck);
        var banditPositions = new List<PositionEvent>();
        foreach (var bandit in bandits)
        {
            banditPositions.AddRange(log.CombatData.GetMovementData(bandit).OfType<PositionEvent>());
        }
        //
        var lastPack = packedTrios.Last();
        var chest = log.AgentData.GetGadgetsByID(ChestID.ChestOfPrisonCamp).FirstOrDefault();
        var hasMultiple = packedTrios.Count > 1;
        var encounterCount = 1;
        foreach (var pack in packedTrios)
        {
            long start = pack.Min(x => x.FirstAware);
            long end = pack.Max(x => x.LastAware);
            var success = false;
            var banditsInBox = banditPositions.Where(x => x.Time < start + 10000 && x.Time > start && x.GetPointXY().IsInBoundingBox(boxStart, boxEnd))
                .Select(x => x.Src)
                .ToHashSet();
            if (banditsInBox.Count > 0)
            {
                start = Math.Min(banditsInBox.Min(x => x.FirstAware), start);
            }
            if (chest != null && pack == lastPack)
            {
                success = true;
                end = chest.FirstAware;
            }
            var phase = new PhaseData(start, end, "Bandit Trio");
            phases.Add(phase);
            if (hasMultiple)
            {
                phase.Name += " " + (encounterCount++);
            }
            if (success)
            {
                phase.Name += " (Success)";
            }
            else
            {
                phase.Name += " (Failure)";
            }
            phase.AddTargets(pack, log);
            phases[0].AddTargets(pack, log);
        }
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.Slothasor, [], ChestID.SlothasorChest, "Slothasor");
        HandleTrioPhases(targetsByIDs, log, phases);
        ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.Matthias, Targets.Where(x => x.IsSpecies(TargetID.MatthiasSacrificeCrystal)), ChestID.MatthiasChest, "Matthias");
        if (phases[0].Targets.Count == 0)
        {
            phases[0].AddTarget(Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Instance)), log);
        }
        return phases;
    }


    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _slothasor.GetInstantCastFinders(),
            .. _banditTrio.GetInstantCastFinders(),
            .. _matthias.GetInstantCastFinders()
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _slothasor.GetTrashMobsIDs(),
            .. _banditTrio.GetTrashMobsIDs(),
            .. _matthias.GetTrashMobsIDs()
        ];
        return trashes.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _slothasor.GetTargetsIDs(),
            .. _banditTrio.GetTargetsIDs(),
            .. _matthias.GetTargetsIDs()
        ];
        targets.Add(TargetID.Instance);
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _slothasor.GetFriendlyNPCIDs(),
            .. _banditTrio.GetFriendlyNPCIDs(),
            .. _matthias.GetFriendlyNPCIDs()
        ];
        return friendlies.Distinct().ToList();
    }


    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        Slothasor.FindMushrooms(fightData, agentData, combatData, extensions);  
        BanditTrio.FindCageAndBombs(agentData, combatData);
        Matthias.FindSacrifices(fightData, agentData, combatData, extensions);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        Matthias.ForceSacrificeHealth(Targets);
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
        foreach (SalvationPass logic in _subLogics)
        {
            logic.ComputeNPCCombatReplayActors(target, log, replay);
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        foreach (SalvationPass logic in _subLogics)
        {
            logic.ComputePlayerCombatReplayActors(p, log, replay);
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        foreach (SalvationPass logic in _subLogics)
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
