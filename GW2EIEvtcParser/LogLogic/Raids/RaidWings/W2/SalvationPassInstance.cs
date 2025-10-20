using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class SalvationPassInstance : SalvationPass
{
    private readonly Slothasor _slothasor;
    private readonly BanditTrio _banditTrio;
    private readonly Matthias _matthias;

    private readonly IReadOnlyList<SalvationPass> _subLogics;
    public SalvationPassInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
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

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Salvation Pass";
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap((800, 1800), (-12288, -27648, 12288, 27648));
        arenaDecorations.Add(new ArenaDecoration((log.LogData.LogStart, log.LogData.LogEnd), CombatReplaySalvationPass, crMap));
        foreach (var subLogic in _subLogics)
        {
            subLogic.GetCombatMapInternal(log, arenaDecorations);
        }
        return crMap;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        var chest = agentData.GetGadgetsByID(_matthias.ChestID).FirstOrDefault();
        if (chest != null)
        {
            logData.SetSuccess(true, chest.FirstAware);
            return;
        }
        base.CheckSuccess(combatData, agentData, logData, playerAgents);
    }

    private List<EncounterPhaseData> HandleTrioPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var packedTrios = new List<List<SingleActor>>();
        if (targetsByIDs.TryGetValue((int)TargetID.Narella, out var narellas))
        {
            if (targetsByIDs.TryGetValue((int)TargetID.Zane, out var zanes))
            {
                if (targetsByIDs.TryGetValue((int)TargetID.Berg, out var bergs))
                {
                    foreach (var narella in narellas)
                    {
                        var pack = new List<SingleActor>(3);
                        pack.Add(narella);
                        var curZane = zanes.FirstOrDefault(x => x.AgentItem.InAwareTimes(narella.AgentItem));
                        if (curZane != null)
                        {
                            pack.Add(curZane);
                        }
                        var curBerg = bergs.FirstOrDefault(x => x.AgentItem.InAwareTimes(narella.AgentItem));
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
        var chest = log.AgentData.GetGadgetsByID(_banditTrio.ChestID).FirstOrDefault();
        var encounterPhases = new List<EncounterPhaseData>();
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
            AddInstanceEncounterPhase(log, phases, encounterPhases, pack, [], [], phases[0], "Bandit Trio", start, end, success, _banditTrio);
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        {

            var slothasorPhases = ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.Slothasor, [], "Slothasor", _slothasor);
            foreach (var slothasorPhase in slothasorPhases)
            {
                var slothasor = slothasorPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Slothasor));
                phases.AddRange(Slothasor.ComputePhases(log, slothasor, slothasorPhase, requirePhases));
            }
        }
        {
            var trioPhases = HandleTrioPhases(targetsByIDs, log, phases);
            foreach (var trioPhase in trioPhases)
            {
                var berg = trioPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Berg));
                var zane = trioPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Zane));
                var narella = trioPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Narella));
                phases.AddRange(BanditTrio.ComputePhases(log, berg, zane, narella, trioPhase, requirePhases));
            }
        }
        {
            var matthiasPhases = ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.Matthias, Targets.Where(x => x.IsSpecies(TargetID.MatthiasSacrificeCrystal)), "Matthias", _matthias);
            foreach (var matthiasPhase in matthiasPhases)
            {
                var matthias = matthiasPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Matthias));
                phases.AddRange(Matthias.ComputePhases(log, matthias, matthiasPhase, requirePhases));
            }
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


    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        Slothasor.FindMushrooms(logData, agentData, combatData, extensions);  
        BanditTrio.FindCageAndBombs(agentData, combatData);
        Matthias.FindSacrifices(logData, agentData, combatData, extensions);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
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

    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        var res = new List<HealthDamageEvent>();
        foreach (var subLogic in _subLogics)
        {
            res.AddRange(subLogic.SpecialDamageEventProcess(combatData, agentData, skillData));
        }
        return res;
    }

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

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        base.SetInstanceBuffs(log, instanceBuffs);
        foreach (SalvationPass logic in _subLogics)
        {
            logic.SetInstanceBuffs(log, instanceBuffs);
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
