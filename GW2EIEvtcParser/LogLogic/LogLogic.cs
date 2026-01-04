using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Decoration;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

public abstract class LogLogic
{

    public enum ParseModeEnum { Instanced50, Instanced10, Instanced5, Benchmark, WvW, sPvP, OpenWorld, Unknown };
    public enum SkillModeEnum { PvE, WvW, sPvP };

    [Flags]
    protected enum FallBackMethod
    {
        None = 0,
        Death = 1 << 0,
        CombatExit = 1 << 1,
    }


    private CombatReplayMap Map;
    protected readonly List<MechanicContainer> MechanicList;//Resurrects (start), Resurrect
    public ParseModeEnum ParseMode { get; protected set; } = ParseModeEnum.Unknown;
    public SkillModeEnum SkillMode { get; protected set; } = SkillModeEnum.PvE;
    public string Extension { get; protected set; } = "";
    public string Icon { get; protected set; } = "";
    private readonly int _basicMechanicsCount;
    public bool HasNoEncounterSpecificMechanics => MechanicList.Count == _basicMechanicsCount;
    public IReadOnlyCollection<AgentItem> TargetAgents { get; protected set; } = [];
    public IReadOnlyCollection<AgentItem> NonSquadFriendlyAgents { get; protected set; } = [];
    public IReadOnlyCollection<AgentItem> TrashMobAgents { get; protected set; } = [];
    public IReadOnlyList<NPC> TrashMobs => _trashMobs;
    public IReadOnlyList<SingleActor> NonSquadFriendlies => _nonSquadFriendlies;
    public IReadOnlyList<SingleActor> Targets => _targets;
    public IReadOnlyList<SingleActor> Hostiles => _hostiles;
    protected List<NPC> _trashMobs { get; private set; } = [];
    protected List<SingleActor> _nonSquadFriendlies { get; private set; } = [];
    protected List<SingleActor> _targets { get; private set; } = [];
    protected List<SingleActor> _hostiles { get; private set; } = [];

    internal bool IsInstance => GenericTriggerID == (int)TargetID.Instance;

    internal readonly Dictionary<string, _DecorationMetadata> DecorationCache = [];

    private CombatReplayDecorationContainer EnvironmentDecorations;
    private readonly CombatReplayDecorationContainer ArenaDecorations;

    public ChestID ChestID { get; protected set; } = ChestID.None;


    public struct InstanceBuff
    {
        public readonly Buff Buff;
        public readonly int Stack;
        public readonly PhaseDataWithMetaData AttachedPhase;
        public InstanceBuff(Buff buff, int stack, PhaseDataWithMetaData phase)
        {
            Buff = buff;
            Stack = stack;
            AttachedPhase = phase;
        }
    }
    protected List<InstanceBuff>? InstanceBuffs { get; private set; } = null;

    public bool Targetless { get; protected set; } = false;
    internal readonly int GenericTriggerID;

    public long LogID { get; protected set; } = LogIDs.Unknown;

    public LogCategories LogCategoryInformation { get; protected set; }
    protected FallBackMethod GenericFallBackMethod;

    protected LogLogic(int triggerID)
    {
        GenericTriggerID = triggerID;
        MechanicList = [
            new MechanicGroup([
                new MechanicGroup(
                    [
                        new PlayerStatusMechanic<DeadEvent>(new MechanicPlotlySetting(Symbols.X, Colors.Black), "Dead", "Dead", "Dead", 0, (log, a) => log.CombatData.GetDeadEvents(a))
                            .UsingNoShowOnTable(),
                        new PlayerStatusMechanic<DownEvent>(new MechanicPlotlySetting(Symbols.Cross, Colors.Red), "Downed", "Downed", "Downed", 0, (log, a) => log.CombatData.GetDownEvents(a))
                            .UsingNoShowOnTable(),
                        new PlayerStatusMechanic<AliveEvent>(new MechanicPlotlySetting(Symbols.Cross, Colors.Green), "Got up", "Got up", "Got up", 0, (log, a) => log.CombatData.GetAliveEvents(a))
                            .UsingNoShowOnTable(),
                    ]
                ),
                new PlayerCastStartMechanic(SkillIDs.Resurrect, new MechanicPlotlySetting(Symbols.CrossOpen,Colors.Teal), "Res", "Res", "Res", 0)
                    .UsingNoShowOnTable(),
                new MechanicGroup(
                    [
                        new PlayerStatusMechanic<DespawnEvent>(new MechanicPlotlySetting(Symbols.X, Colors.LightGrey), "DC", "DC", "DC", 0, (log, a) => log.CombatData.GetDespawnEvents(a))
                            .UsingNoShowOnTable(),
                        new PlayerStatusMechanic<SpawnEvent>(new MechanicPlotlySetting(Symbols.Cross, Colors.LightBlue), "Resp", "Resp", "Resp", 0, (log, a) => log.CombatData.GetSpawnEvents(a))
                            .UsingNoShowOnTable()
                    ]
                ),
                new MechanicGroup(
                    [
                        new PlayerDstCrowdControlMechanic(SkillIDs.ArcDPSGenericKnockdown, new MechanicPlotlySetting(Symbols.StarTriangleUp, Colors.Brown), "Knck.Dwn", "Knocked Down", "Knocked Down", 0),
                        new PlayerDstCrowdControlMechanic(SkillIDs.ArcDPSGenericKnockbackPull, new MechanicPlotlySetting(Symbols.StarTriangleUp, Colors.DarkGreen), "Knck.Pll", "Knocked Back or Pulled", "Knocked Back/Pulled", 0),
                        new PlayerDstCrowdControlMechanic(SkillIDs.ArcDPSGenericFloat, new MechanicPlotlySetting(Symbols.StarTriangleUp, Colors.LightBlue), "Flt", "Float", "Float", 0),
                        new PlayerDstCrowdControlMechanic(SkillIDs.ArcDPSGenericLaunch, new MechanicPlotlySetting(Symbols.StarTriangleUp, Colors.DarkPurple), "Lnch", "Launched", "Launched", 0),
                        new PlayerDstCrowdControlMechanic(SkillIDs.ArcDPSGenericWaterFloatSink, new MechanicPlotlySetting(Symbols.StarTriangleUp, Colors.DarkBlue), "Wtr.Flt.Snk", "Float or Sinked in Water", "Float or Sinked", 0),
                    ]
                ),
            ])
        ];
        ArenaDecorations = new(DecorationCache);
        _basicMechanicsCount = MechanicList.Count;
        LogCategoryInformation = new LogCategories();
        GenericFallBackMethod = IsInstance ? FallBackMethod.None : FallBackMethod.Death;
    }

    internal MechanicData GetMechanicData()
    {
        var allMechs = new List<Mechanic>();
        foreach (MechanicContainer mechGroup in MechanicList)
        {
            allMechs.AddRange(mechGroup.GetMechanics());
        }
        return new MechanicData(allMechs, IsInstance);
    }

    internal virtual CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        return new CombatReplayMap((800, 800), (0, 0, 0, 0)/*, (0, 0, 0, 0), (0, 0, 0, 0)*/);
    }

    public CombatReplayMap GetCombatReplayMap(ParsedEvtcLog log)
    {
        if (Map == null)
        {
            Map = GetCombatMapInternal(log, ArenaDecorations);
            Map.ComputeBoundingBox(log);
        }
        return Map;
    }

    internal virtual void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {

    }

    internal virtual void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        var mainPhase = log.LogData.GetMainPhase(log);
        foreach (Buff fractalInstability in log.Buffs.BuffsBySource[Source.FractalInstability])
        {
            if (log.CombatData.GetBuffData(fractalInstability.ID).Any(x => x.To.IsPlayer))
            {
                instanceBuffs.Add(new(fractalInstability, 1, mainPhase));
            }
        }
        var encounterPhases = log.LogData.GetPhases(log).OfType<EncounterPhaseData>();
        foreach (var encounterPhase in encounterPhases)
        {
            long end = encounterPhase.Success ? encounterPhase.End : (encounterPhase.End + encounterPhase.Start) / 2;

            // Emboldened
            int emboldenedStacks = (int)log.PlayerList.Select(x =>
            {
                if (x.GetBuffGraphs(log).TryGetValue(SkillIDs.Emboldened, out var graph))
                {
                    return graph.Values.Where(y => y.Intersects(encounterPhase.Start, end)).Max(y => y.Value);
                }
                else
                {
                    return 0;
                }
            }).Max();
            if (emboldenedStacks > 0)
            {
                instanceBuffs.Add(new(log.Buffs.BuffsByIDs[SkillIDs.Emboldened], emboldenedStacks, mainPhase));
            }
        }
        // Quickplay
        var hasQuickplay = log.PlayerList.Any(x => x.HasBuff(log, SkillIDs.QuickplayBoost, log.LogData.LogStart, log.LogData.LogEnd));
        if (hasQuickplay)
        {
            instanceBuffs.Add(new(log.Buffs.BuffsByIDs[SkillIDs.QuickplayBoost], 1, mainPhase));
        }
    }

    public IReadOnlyList<InstanceBuff> GetInstanceBuffs(ParsedEvtcLog log)
    {
        if (InstanceBuffs == null)
        {
            InstanceBuffs = [];
            SetInstanceBuffs(log, InstanceBuffs);
        }
        return InstanceBuffs;
    }

    internal virtual int GetTriggerID()
    {
        return GenericTriggerID;
    }

    internal abstract IReadOnlyList<TargetID> GetTargetsIDs();

    internal virtual HashSet<TargetID> ForbidBreakbarPhasesFor()
    {
        return [];
    }

    internal virtual Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        var targetsIDs = GetTargetsIDs();
        var res = new Dictionary<TargetID, int>(targetsIDs.Count);
        for (int i = 0; i < targetsIDs.Count; i++)
        {
            res.Add(targetsIDs[i], i);
        }
        return res;
    }

    internal virtual IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return [ ];
    }

    internal virtual IReadOnlyList<TargetID>  GetFriendlyNPCIDs()
    {
        return [ ];
    }

    internal virtual string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        SingleActor? target = Targets.FirstOrDefault(x => x.IsSpecies(GenericTriggerID));
        if (target == null)
        {
            return "UNKNOWN";
        }
        return target.Character;
    }

    protected virtual HashSet<int> IgnoreForAutoNumericalRenaming()
    {
        return [];
    }

    private void ComputeLogTargets(AgentData agentData, List<CombatItem> combatItems, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var ignoredSpeciesForRenaming = IgnoreForAutoNumericalRenaming();

        // Build targets
        //NOTE(Rennorb): Even though this collection is used for contains tests, it is still faster to just iterate the 5 or so members this can have than
        // to build the hashset and hash the value each time.
        var targetIDs = GetTargetsIDs();
        //NOTE(Rennorb): Even though this collection is used for contains tests, it is still faster to just iterate the 5 or so members this can have than
        // to build the hashset and hash the value each time.
        _targets.AddRange(agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.IsAnySpecies(targetIDs) && x.LastAware > 0).Select(a => new NPC(a)));
        if (IsInstance)
        {
            _targets.AddRange(agentData.GetNPCsByID(TargetID.Instance).Select(a => new NPC(a)));
        }
        _targets.SortByFirstAware();
        var targetSortIDs = GetTargetsSortIDs();
        //TODO_PERF(Rennorb)
        _targets = _targets.OrderBy(x =>
        {
            if (targetSortIDs.TryGetValue(GetTargetID(x.ID), out int sortKey))
            {
                return sortKey;
            }
            return int.MaxValue;
        }).ToList();
        NumericallyRenameSpecies(Targets, ignoredSpeciesForRenaming);

        // Build trash mobs
        var trashIDs = GetTrashMobsIDs();
        foreach (var trash in trashIDs)
        {
            if(targetIDs.IndexOf(trash) != -1)
            {
                throw new InvalidDataException("ID collision between trash and targets: " + nameof(trash));
            }
        }
        _trashMobs.AddRange(agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.IsAnySpecies(trashIDs) && x.LastAware > 0).Select(a => new NPC(a)));
        //aList.AddRange(agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => ids2.Contains(ParseEnum.GetTrashIDS(x.ID))));
#if DEBUG2
        var unknownAList = agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.InstID != 0 && x.LastAware - x.FirstAware > 1000 && !trashIDs.Contains(GetTargetID(x.ID)) && !targetIDs.Contains(GetTargetID(x.ID)) && !x.GetFinalMaster().IsPlayer).ToList();
        unknownAList.AddRange(agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => x.LastAware - x.FirstAware > 1000 && !x.GetFinalMaster().IsPlayer));
        foreach (AgentItem a in unknownAList)
        {
            _trashMobs.Add(new NPC(a));
        }
#endif
        _trashMobs.SortByFirstAware();
        NumericallyRenameSpecies(TrashMobs, ignoredSpeciesForRenaming);

        // Build friendlies
        var friendlyIDs = GetFriendlyNPCIDs();
        _nonSquadFriendlies.AddRange(agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.IsAnySpecies(friendlyIDs) && x.LastAware > 0).Select(a => new NPC(a)));
        _nonSquadFriendlies.SortByFirstAware();
        NumericallyRenameSpecies(NonSquadFriendlies, ignoredSpeciesForRenaming);

        FinalizeComputeLogTargets();
    }

    internal virtual void UpdatePlayersSpecAndGroup(IReadOnlyList<Player> players, CombatData combatData, LogData logData)
    {
        //
        if (IsInstance || ParseMode == ParseModeEnum.WvW)
        {
            foreach (Player p in players)
            {
                // We get the first enter combat for the player, we ignore it however if there was an exit combat before it as that means the player was already in combat at log start
                var enterCombat = combatData.GetEnterCombatEvents(p.AgentItem).FirstOrDefault(x => x.Spec != Spec.Unknown && x.Subgroup != 0);
                if (enterCombat != null && enterCombat.Subgroup > 0 && !combatData.GetExitCombatEvents(p.AgentItem).Any(x => x.Time < enterCombat.Time))
                {
                    p.AgentItem.OverrideSpec(enterCombat.Spec);
                    p.OverrideGroup(enterCombat.Subgroup);
                }
            }
            return;
        }
        long threshold = logData.LogStart + 5000;
        foreach (Player p in players)
        {
            EnterCombatEvent? enterCombat = null;
            if (p.FirstAware > threshold)
            {
                enterCombat = combatData.GetEnterCombatEvents(p.AgentItem).FirstOrDefault(x => x.Spec != Spec.Unknown && x.Subgroup != 0);
            } 
            else
            {
                enterCombat = combatData.GetEnterCombatEvents(p.AgentItem).Where(x => x.Time <= threshold).LastOrDefault(x => x.Spec != Spec.Unknown && x.Subgroup != 0);
            }
            if (enterCombat != null)
            {
                p.AgentItem.OverrideSpec(enterCombat.Spec);
                p.OverrideGroup(enterCombat.Subgroup);
            }
        }
    }

    protected void FinalizeComputeLogTargets()
    {
        TargetAgents = new HashSet<AgentItem>(_targets.Select(x => x.AgentItem));
        NonSquadFriendlyAgents = new HashSet<AgentItem>(_nonSquadFriendlies.Select(x => x.AgentItem));
        TrashMobAgents = new HashSet<AgentItem>(_trashMobs.Select(x => x.AgentItem));
        _hostiles = [.. _targets, .. _trashMobs];
    }

    internal virtual List<InstantCastFinder> GetInstantCastFinders()
    {
        return [ ];
    }

    internal void InvalidateLogID()
    {
        LogID = LogIDs.LogMasks.Unsupported;
    }

    internal List<PhaseData> GetBreakbarPhases(ParsedEvtcLog log, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [ ];
        }

        //TODO_PERF(Rennorb): find average complexity
        var breakbarPhases = new List<PhaseData>(Targets.Count);
        var noBreakbarSpecies = ForbidBreakbarPhasesFor();
        foreach (SingleActor target in Targets)
        {
            if (target.IsAnySpecies(noBreakbarSpecies) || target.AgentItem.IsPlayer)
            {
                continue;
            }
            int i = 0;
            var (_, breakbarActives, _, _) = target.GetBreakbarStatus(log);
            var (_, _, _, actives) = target.GetStatus(log);
            foreach (Segment breakbarActive in breakbarActives)
            {
                if (Math.Abs(breakbarActive.End - breakbarActive.Start) < ServerDelayConstant || !actives.Any(x => x.Intersects(breakbarActive)))
                {
                    continue;
                }

                long start = Math.Max(breakbarActive.Start - BreakbarPhaseTimeBuildup, log.LogData.LogStart);
                long end = Math.Min(breakbarActive.End, log.LogData.LogEnd);
                var phase = new BreakbarPhaseData(start, end, target.Character + " Breakbar " + ++i);
                phase.AddTarget(target, log);
                breakbarPhases.Add(phase);
            }
        }
        return breakbarPhases;
    }

    internal virtual List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        if (IsInstance)
        {
            var targets = Targets.Where(x => x.GetHealth(log.CombatData) > 3e6 && x.LastAware - x.FirstAware > MinimumInCombatDuration);
            if (targets.Any())
            {
                AddPhasesPerTarget(log, phases, targets);
            }
            else
            {
                phases[0].AddTargets(Targets.Where(x => x.IsSpecies(TargetID.Instance)), log);
            }
        } 
        else
        {
            phases[0].AddTargets(Targets.Where(x => x.IsSpecies(GenericTriggerID)), log);
        }
        return phases;
    }

    internal virtual IEnumerable<ErrorEvent> GetCustomWarningMessages(LogData logData, AgentData agentData, CombatData combatData, EvtcVersionEvent evtcVersion)
    {
        if (evtcVersion.Build >= ArcDPSBuilds.DirectX11Update)
        {
            return [new("As of arcdps 20210923, animated cast events' durations are broken, as such, any feature having a dependency on it are to be taken with a grain of salt. Impacted features are: <br>- Rotations <br>- Time spent in animation statistics <br>- Mechanics <br>- Phases <br>- Combat Replay Decorations")];
        }
        return [];
    }


    protected static void AddTargetsToPhase(PhaseData phase, IReadOnlyList<SingleActor> targets, IReadOnlyList<TargetID> ids, ParsedEvtcLog log, PhaseData.TargetPriority priority = PhaseData.TargetPriority.Main)
    {
        foreach (SingleActor target in targets)
        {
            if (target.IsAnySpecies(ids))
            {
                phase.AddTarget(target, log, priority);
            }
        }
    }
    protected void AddTargetsToPhase(PhaseData phase, IReadOnlyList<TargetID> ids, ParsedEvtcLog log, PhaseData.TargetPriority priority = PhaseData.TargetPriority.Main)
    {
        AddTargetsToPhase(phase, Targets, ids, log, priority);
    }

    protected static void AddTargetsToPhaseAndFit(PhaseData phase, IReadOnlyList<SingleActor> targets, IReadOnlyList<TargetID> ids, ParsedEvtcLog log, PhaseData.TargetPriority priority = PhaseData.TargetPriority.Main)
    {
        AddTargetsToPhase(phase, targets, ids, log, priority);
        phase.OverrideTimes(log);
    }

    protected void AddTargetsToPhaseAndFit(PhaseData phase, IReadOnlyList<TargetID> ids, ParsedEvtcLog log, PhaseData.TargetPriority priority = PhaseData.TargetPriority.Main)
    {
        AddTargetsToPhaseAndFit(phase, Targets, ids, log, priority);
    }

    internal virtual List<BuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
    {
        return [ ];
    }

    internal virtual List<CastEvent> SpecialCastEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        return [ ];
    }

    internal virtual List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        return [ ];
    }

    internal virtual void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
    }

    internal virtual void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
    }

    internal virtual void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        IEnumerable<SquadMarkerIndex> squadMarkers = [
            SquadMarkerIndex.Arrow,
            SquadMarkerIndex.Circle,
            SquadMarkerIndex.Heart,
            SquadMarkerIndex.Square,
            SquadMarkerIndex.Star,
            SquadMarkerIndex.Swirl,
            SquadMarkerIndex.Triangle,
            SquadMarkerIndex.X,
        ];
        foreach (var squadMarker in squadMarkers)
        {
            foreach (var squadMarkerEvent in log.CombatData.GetSquadMarkerEvents(squadMarker))
            {
                if (ParserIcons.SquadMarkerIndexToIcon.TryGetValue(squadMarker, out var icon))
                {
                    environmentDecorations.Add(new IconDecoration(icon, 16, 90, 0.8f, (squadMarkerEvent.Time, squadMarkerEvent.EndTime), new PositionConnector(squadMarkerEvent.Position)).UsingSquadMarker(true));
                }
            }
        }
    }

    internal IReadOnlyList<DecorationRenderingDescription> GetCombatReplayArenaDecorationRenderableDescriptions(CombatReplayMap map, ParsedEvtcLog log)
    {
        return ArenaDecorations.GetCombatReplayRenderableDescriptions(Map, log, [], []);
    }

    internal IReadOnlyList<DecorationRenderingDescription> GetCombatReplayEnvironmentDecorationRenderableDescriptions(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        if (EnvironmentDecorations == null)
        {
            //TODO_PERF(Rennorb): capacity
            EnvironmentDecorations = new(DecorationCache);
            ComputeEnvironmentCombatReplayDecorations(log, EnvironmentDecorations);
        }
        return EnvironmentDecorations.GetCombatReplayRenderableDescriptions(map, log, usedSkills, usedBuffs);
    }

    internal virtual LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        if (IsInstance)
        {
            return LogData.LogMode.NotApplicable;
        }
        return LogData.LogMode.Normal;
    }

    internal virtual LogData.LogStartStatus GetLogStartStatus(CombatData combatData, AgentData agentData, LogData logData)
    {
        if (IsInstance)
        {
            InstanceStartEvent? evt = combatData.GetInstanceStartEvent();
            if (evt == null)
            {
                return LogData.LogStartStatus.Normal;
            }
            else
            {
                return evt.TimeOffsetFromInstanceCreation > 10000 ? LogData.LogStartStatus.Late : LogData.LogStartStatus.Normal;
            }
        }
        return LogData.LogStartStatus.Normal;
    }

    protected virtual IReadOnlyList<TargetID> GetSuccessCheckIDs()
    {
        return [ GetTargetID(GenericTriggerID) ];
    }

    internal virtual void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        NoBouncyChestGenericCheckSucess(combatData, agentData, logData, playerAgents);
    }

    protected IEnumerable<SingleActor> GetSuccessCheckTargets()
    {
        return Targets.Where(x => x.IsAnySpecies(GetSuccessCheckIDs()));
    }

    protected void NoBouncyChestGenericCheckSucess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        if (!logData.Success && ChestID != ChestID.None)
        {
            SetSuccessByChestGadget(ChestID, agentData, logData);
        }
        if (!logData.Success && (GenericFallBackMethod & FallBackMethod.Death) > 0)
        {
            SetSuccessByDeath(GetSuccessCheckTargets(), combatData, logData, playerAgents, true);
        }
        if (!logData.Success && (GenericFallBackMethod & FallBackMethod.CombatExit) > 0)
        {
            SetSuccessByCombatExit(GetSuccessCheckTargets(), combatData, logData, playerAgents);
        }
        if (!logData.Success)
        {
            var targets = GetSuccessCheckTargets();
            if (targets.Any())
            {
                logData.SetSuccess(false, targets.Max(x => x.LastAware));
            }
        }
    }

    /// <summary>
    /// To be used in situations where stabilisation or identification of a certain species id is necessary ASAP
    /// </summary>
    internal virtual void HandleCriticalAgents(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {

    }

    internal virtual long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericLogOffset(logData);
        if  (IsInstance)
        {
            return startToUse;
        }
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            startToUse = GetEnterCombatTime(logData, agentData, combatData, logStartNPCUpdate.Time, GenericTriggerID, logStartNPCUpdate.DstAgent);
        }
        return startToUse;
    }

    internal virtual LogLogic AdjustLogic(AgentData agentData, List<CombatItem> combatData, EvtcParserSettings parserSettings)
    {
        return this;
    }

    internal virtual void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (IsInstance)
        {
            agentData.AddCustomNPCAgent(logData.LogStart, logData.LogEnd, "Dummy Instance", Spec.NPC, TargetID.Instance, true);
        }
        ComputeLogTargets(agentData, combatData, extensions);
        if (IsInstance && Targets.Count == Targets.Count(x => x.IsSpecies(TargetID.Instance)))
        {
            Targetless = true;
        }
    }

    /// <summary>
    /// The buff must be present on any player at the end of the encounter.<br></br>
    /// </summary>
    /// <param name="stack">Amount of buff stacks (0-99).</param>
    /// <returns>
    /// A pari of (<paramref name="buff"/> and its <paramref name="stack"/>) if present, otherwise null.<br></br>
    /// To be used to add to <see cref="InstanceBuffs"/>. Use <see cref="ListExt.MaybeAdd"/>.
    /// </returns>
    protected static InstanceBuff? GetOnPlayerCustomInstanceBuff(ParsedEvtcLog log, PhaseDataWithMetaData encounterPhase, long buff, int stack = 1)
    {
        foreach (Player p in log.PlayerList)
        {
            if (p.HasBuff(log, buff, encounterPhase.End - ServerDelayConstant))
            {
                return new(log.Buffs.BuffsByIDs[buff], stack, encounterPhase);
            }
        }
        return null;
    }

    /// <summary>
    /// Determinate the privaty state of an instance.
    /// </summary>
    internal virtual LogData.InstancePrivacyMode GetInstancePrivacyMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.InstancePrivacyMode.NotApplicable;
    }
}
