using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Decoration;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

public abstract class FightLogic
{

    public enum ParseModeEnum { FullInstance, Instanced10, Instanced5, Benchmark, WvW, sPvP, OpenWorld, Unknown };
    public enum SkillModeEnum { PvE, WvW, sPvP };

    [Flags]
    protected enum FallBackMethod
    {
        None = 0,
        Death = 1 << 0,
        CombatExit = 1 << 1,
        ChestGadget = 1 << 2
    }


    private CombatReplayMap _map;
    protected readonly List<Mechanic> MechanicList;//Resurrects (start), Resurrect
    public ParseModeEnum ParseMode { get; protected set; } = ParseModeEnum.Unknown;
    public SkillModeEnum SkillMode { get; protected set; } = SkillModeEnum.PvE;
    public string Extension { get; protected set; }
    public string Icon { get; protected set; }
    private readonly int _basicMechanicsCount;
    public bool HasNoFightSpecificMechanics => MechanicList.Count == _basicMechanicsCount;
    public IReadOnlyCollection<AgentItem> TargetAgents { get; protected set; }
    public IReadOnlyCollection<AgentItem> NonPlayerFriendlyAgents { get; protected set; }
    public IReadOnlyCollection<AgentItem> TrashMobAgents { get; protected set; }
    public IReadOnlyList<NPC> TrashMobs => _trashMobs;
    public IReadOnlyList<SingleActor> NonPlayerFriendlies => _nonPlayerFriendlies;
    public IReadOnlyList<SingleActor> Targets => _targets;
    public IReadOnlyList<SingleActor> Hostiles => _hostiles;
    protected List<NPC> _trashMobs { get; private set; } = [];
    protected List<SingleActor> _nonPlayerFriendlies { get; private set; } = [];
    protected List<SingleActor> _targets { get; private set; } = [];
    protected List<SingleActor> _hostiles { get; private set; } = [];

    internal readonly Dictionary<string, _DecorationMetadata> DecorationCache = [];

    internal CombatReplayDecorationContainer EnvironmentDecorations;

    protected ChestID ChestID = ChestID.None;

    protected List<(Buff buff, int stack)>? InstanceBuffs { get; private set; } = null;

    public bool Targetless { get; protected set; } = false;
    internal readonly int GenericTriggerID;

    public long EncounterID { get; protected set; } = EncounterIDs.Unknown;

    public EncounterCategory EncounterCategoryInformation { get; protected set; }
    protected FallBackMethod GenericFallBackMethod = FallBackMethod.Death;

    protected FightLogic(int triggerID)
    {
        GenericTriggerID = triggerID;
        MechanicList = [
            new PlayerStatusMechanic<DeadEvent>("Dead", new MechanicPlotlySetting(Symbols.X, Colors.Black), "Dead", "Dead", "Dead", 0, (log, a) => log.CombatData.GetDeadEvents(a)).UsingShowOnTable(false),
            new PlayerStatusMechanic<DownEvent>("Downed", new MechanicPlotlySetting(Symbols.Cross, Colors.Red), "Downed", "Downed", "Downed", 0, (log, a) => log.CombatData.GetDownEvents(a)).UsingShowOnTable(false),
            new PlayerCastStartMechanic(SkillIDs.Resurrect, "Resurrect", new MechanicPlotlySetting(Symbols.CrossOpen,Colors.Teal), "Res", "Res", "Res",0).UsingShowOnTable(false),
            new PlayerStatusMechanic<AliveEvent>("Got up", new MechanicPlotlySetting(Symbols.Cross, Colors.Green), "Got up", "Got up", "Got up", 0, (log, a) => log.CombatData.GetAliveEvents(a)).UsingShowOnTable(false),
            new PlayerStatusMechanic<DespawnEvent>("Disconnected", new MechanicPlotlySetting(Symbols.X, Colors.LightGrey), "DC", "DC", "DC", 0, (log, a) => log.CombatData.GetDespawnEvents(a)).UsingShowOnTable(false),
            new PlayerStatusMechanic<SpawnEvent>("Respawn", new MechanicPlotlySetting(Symbols.Cross, Colors.LightBlue), "Resp", "Resp", "Resp", 0, (log, a) => log.CombatData.GetSpawnEvents(a)).UsingShowOnTable(false)
        ];
        _basicMechanicsCount = MechanicList.Count;
        EncounterCategoryInformation = new EncounterCategory();
    }

    internal MechanicData GetMechanicData()
    {
        return new MechanicData(MechanicList);
    }

    protected virtual CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap("", (800, 800), (0, 0, 0, 0)/*, (0, 0, 0, 0), (0, 0, 0, 0)*/);
    }

    public CombatReplayMap GetCombatReplayMap(ParsedEvtcLog log)
    {
        if (_map == null)
        {
            _map = GetCombatMapInternal(log);
            _map.ComputeBoundingBox(log);
        }
        return _map;
    }

    [MemberNotNull(nameof(InstanceBuffs))]
    protected virtual void SetInstanceBuffs(ParsedEvtcLog log)
    {
        InstanceBuffs = [];
        foreach (Buff fractalInstability in log.Buffs.BuffsBySource[Source.FractalInstability])
        {
            if (log.CombatData.GetBuffData(fractalInstability.ID).Any(x => x.To.IsPlayer))
            {
                InstanceBuffs.Add((fractalInstability, 1));
            }
        }
        long end = log.FightData.Success ? log.FightData.FightEnd : (log.FightData.FightEnd + log.FightData.FightStart) / 2;
        int emboldenedStacks = (int)log.PlayerList.Select(x =>
        {
            if (x.GetBuffGraphs(log).TryGetValue(SkillIDs.Emboldened, out var graph))
            {
                return graph.Values.Where(y => y.Intersects(log.FightData.FightStart, end)).Max(y => y.Value);
            }
            else
            {
                return 0;
            }
        }).Max();
        if (emboldenedStacks > 0)
        {
            InstanceBuffs.Add((log.Buffs.BuffsByIds[SkillIDs.Emboldened], emboldenedStacks));
        }
    }

    public virtual IReadOnlyList<(Buff buff, int stack)> GetInstanceBuffs(ParsedEvtcLog log)
    {
        if (InstanceBuffs == null)
        {
            SetInstanceBuffs(log);
        }
        return InstanceBuffs;
    }

    internal virtual int GetTriggerID()
    {
        return GenericTriggerID;
    }

    /// <remarks>Do _NOT_ modify Instance._targetIDs while iterating the result of this function. Appending is allowed.</remarks>
    protected virtual ReadOnlySpan<int> GetTargetsIDs()
    {
        return new[] { GenericTriggerID };
    }

    protected virtual Dictionary<int, int> GetTargetsSortIDs()
    {
        var targetsIds = GetTargetsIDs();
        var res = new Dictionary<int, int>(targetsIds.Length);
        for (int i = 0; i < targetsIds.Length; i++)
        {
            res.Add(targetsIds[i], i);
        }
        return res;
    }

    //TODO(Rennorb) @cleanup: use readonlyspan? 
    //NOTE(Rennorb): I purposefully did not change this to a span or array for now, because there are quite a few overrides that take the shape of
    /*
    protected virtual List<TrashID> GetTrashMobsIDs()
    {
        var trash = new List<>() {A, B};
        trash.AddRange(base.GetTrashMobsIDs);
        return trash;
    }
    */
    // changing the return type to a span is still possible, but initialization requires them to be rewritten with manual array indices and sizes.
    // This is likely to cause issues in the future, because someone _will_ miss updating the indices correctly is something gets added.
    // On the other hand i don't know how often the lists even change, i would imagine this to not happen very frequently - so it still might be a thing we could do.
    protected virtual List<TrashID> GetTrashMobsIDs()
    {
        return [ ];
    }

    protected virtual ReadOnlySpan<int> GetFriendlyNPCIDs()
    {
        return [ ];
    }

    internal virtual string GetLogicName(CombatData combatData, AgentData agentData)
    {
        SingleActor? target = Targets.FirstOrDefault(x => x.IsSpecies(GenericTriggerID));
        if (target == null)
        {
            return "UNKNOWN";
        }
        return target.Character;
    }

    protected abstract ReadOnlySpan<int> GetUniqueNPCIDs();

    private void ComputeFightTargets(AgentData agentData, List<CombatItem> combatItems, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        RegroupSameInstidNPCsByID(GetUniqueNPCIDs(), agentData, combatItems, extensions);

        //NOTE(Rennorb): Even though this collection is used for contains tests, it is still faster to just iterate the 5 or so members this can have than
        // to build the hashset and hash the value each time.
        var targetIDs = GetTargetsIDs().ToArray();
        var trashIDs = GetTrashMobsIDs();
        //NOTE(Rennorb): Even though this collection is used for contains tests, it is still faster to just iterate the 5 or so members this can have than
        // to build the hashset and hash the value each time.

// Build targets
#if !DEBUG2
        foreach (int id in targetIDs)
        {
            IReadOnlyList<AgentItem> agents = agentData.GetNPCsByID(id);
            foreach (AgentItem agentItem in agents)
            {
                _targets.Add(new NPC(agentItem));
            }
        }
#else
        _targets.AddRange(agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => !trashIDs.Contains(GetTrashID(x.ID)) && !x.GetFinalMaster().IsPlayer && !x.IsNonIdentifiedSpecies()).Select(a => new NPC(a)));
#endif
        //TODO(Rennorb) @perf @cleanup: is this required?
        _targets.SortByFirstAware();

        var targetSortIDs = GetTargetsSortIDs();
        //TODO(Rennorb) @perf
        _targets = _targets.OrderBy(x =>
        {
            if (targetSortIDs.TryGetValue(x.ID, out int sortKey))
            {
                return sortKey;
            }
            return int.MaxValue;
        }).ToList();
        // Build trash mobs
        foreach (var trash in trashIDs)
        {
            if(targetIDs.IndexOf((int)trash) != -1)
            {
                throw new InvalidDataException("ID collision between trash and targets: " + nameof(trash));
            }
        }
        _trashMobs.AddRange(agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => trashIDs.Contains(GetTrashID(x.ID))).Select(a => new NPC(a)));
        //aList.AddRange(agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => ids2.Contains(ParseEnum.GetTrashIDS(x.ID))));
#if DEBUG2
        var unknownAList = agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.InstID != 0 && x.LastAware - x.FirstAware > 1000 && !trashIDs.Contains(GetTrashID(x.ID)) && !targetIDs.Contains(x.ID) && !x.GetFinalMaster().IsPlayer).ToList();
        unknownAList.AddRange(agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => x.LastAware - x.FirstAware > 1000 && !x.GetFinalMaster().IsPlayer));
        foreach (AgentItem a in unknownAList)
        {
            _trashMobs.Add(new NPC(a));
        }
#endif
        _trashMobs.SortByFirstAware();

        foreach (int id in GetFriendlyNPCIDs())
        {
            _nonPlayerFriendlies.AddRange(agentData.GetNPCsByID(id).Select(a => new NPC(a)));
        }
        _nonPlayerFriendlies.SortByFirstAware();
        FinalizeComputeFightTargets();
    }

    internal virtual void UpdatePlayersSpecAndGroup(IReadOnlyList<Player> players, CombatData combatData, FightData fightData)
    {
        foreach (Player p in players)
        {
            long threshold = fightData.FightStart + 5000;
            EnterCombatEvent? enterCombat = null;
            if (p.FirstAware > threshold)
            {
                enterCombat = combatData.GetEnterCombatEvents(p.AgentItem).FirstOrDefault();
            } 
            else
            {
                enterCombat = combatData.GetEnterCombatEvents(p.AgentItem).Where(x => x.Time <= threshold).LastOrDefault();
            }
            if (enterCombat != null && enterCombat.Spec != Spec.Unknown && enterCombat.Subgroup != 0)
            {
                p.AgentItem.OverrideSpec(enterCombat.Spec);
                p.OverrideGroup(enterCombat.Subgroup);
            }
        }
    }

    protected void FinalizeComputeFightTargets()
    {
        TargetAgents = new HashSet<AgentItem>(_targets.Select(x => x.AgentItem));
        NonPlayerFriendlyAgents = new HashSet<AgentItem>(_nonPlayerFriendlies.Select(x => x.AgentItem));
        TrashMobAgents = new HashSet<AgentItem>(_trashMobs.Select(x => x.AgentItem));
        _hostiles.AddRange(_targets);
        _hostiles.AddRange(_trashMobs);
    }

    internal virtual List<InstantCastFinder> GetInstantCastFinders()
    {
        return [ ];
    }

    internal void InvalidateEncounterID()
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
    }

    internal List<PhaseData> GetBreakbarPhases(ParsedEvtcLog log, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [ ];
        }

        //TODO(Rennorb) @perf: find average complexity
        var breakbarPhases = new List<PhaseData>(Targets.Count);
        foreach (SingleActor target in Targets)
        {
            int i = 0;
            var (_, breakbarActives, _, _) = target.GetBreakbarStatus(log);
            var (_, _, _, actives) = target.GetStatus(log);
            foreach (Segment breakbarActive in breakbarActives)
            {
                if (Math.Abs(breakbarActive.End - breakbarActive.Start) < ServerDelayConstant || !actives.Any(x => x.Intersects(breakbarActive)))
                {
                    continue;
                }

                long start = Math.Max(breakbarActive.Start - 2000, log.FightData.FightStart);
                long end = Math.Min(breakbarActive.End, log.FightData.FightEnd);
                var phase = new PhaseData(start, end, target.Character + " Breakbar " + ++i)
                {
                    BreakbarPhase = true,
                    CanBeSubPhase = false
                };
                phase.AddTarget(target);
                breakbarPhases.Add(phase);
            }
        }
        return breakbarPhases;
    }

    internal virtual List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(GenericTriggerID)) ?? throw new MissingKeyActorsException("Main target of the fight not found");
        phases[0].AddTarget(mainTarget);
        return phases;
    }

    internal virtual IEnumerable<ErrorEvent> GetCustomWarningMessages(FightData fightData, EvtcVersionEvent evtcVersion)
    {
        if (evtcVersion.Build >= ArcDPSBuilds.DirectX11Update)
        {
            return [ new("As of arcdps 20210923, animated cast events' durations are broken, as such, any feature having a dependency on it are to be taken with a grain of salt. Impacted features are: <br>- Rotations <br>- Time spent in animation statistics <br>- Mechanics <br>- Phases <br>- Combat Replay Decorations") ];
        }
        return [ ];
    }

    protected void AddTargetsToPhase(PhaseData phase, List<int> ids, PhaseData.TargetPriority priority = PhaseData.TargetPriority.Main)
    {
        foreach (SingleActor target in Targets)
        {
            if (ids.Contains(target.ID))
            {
                phase.AddTarget(target, priority);
            }
        }
    }

    protected void AddTargetsToPhaseAndFit(PhaseData phase, List<int> ids, ParsedEvtcLog log, PhaseData.TargetPriority priority = PhaseData.TargetPriority.Main)
    {
        AddTargetsToPhase(phase, ids, priority);
        phase.OverrideTimes(log);
    }

    internal virtual List<BuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
    {
        return [ ];
    }

    internal virtual List<CastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
    {
        return [ ];
    }

    internal virtual List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, SkillData skillData)
    {
        return [ ];
    }

    internal virtual void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
    }

    internal virtual void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
    }

    internal virtual void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
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
                    EnvironmentDecorations.Add(new IconDecoration(icon, 16, 90, 0.8f, (squadMarkerEvent.Time, squadMarkerEvent.EndTime), new PositionConnector(squadMarkerEvent.Position)).UsingSquadMarker(true));
                }
            }
        }
    }

    internal IReadOnlyList<DecorationRenderingDescription> GetCombatReplayDecorationRenderableDescriptions(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        if (EnvironmentDecorations == null)
        {
            //TODO(Rennorb) @perf: capacity
            EnvironmentDecorations = new(DecorationCache);
            ComputeEnvironmentCombatReplayDecorations(log);
        }
        return EnvironmentDecorations.GetCombatReplayRenderableDescriptions(map, log, usedSkills, usedBuffs);
    }

    internal virtual FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.EncounterMode.Normal;
    }

    internal virtual FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.EncounterStartStatus.Normal;
    }

    protected virtual List<int> GetSuccessCheckIDs()
    {
        return [ GenericTriggerID ];
    }

    internal virtual void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        NoBouncyChestGenericCheckSucess(combatData, agentData, fightData, playerAgents);
    }

    protected IEnumerable<SingleActor> GetSuccessCheckTargets()
    {
        return Targets.Where(x => GetSuccessCheckIDs().Contains(x.ID));
    }

    protected void NoBouncyChestGenericCheckSucess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        if (!fightData.Success && (GenericFallBackMethod & FallBackMethod.ChestGadget) > 0)
        {
            SetSuccessByChestGadget(ChestID, agentData, fightData);
        }
        if (!fightData.Success && (GenericFallBackMethod & FallBackMethod.Death) > 0)
        {
            SetSuccessByDeath(GetSuccessCheckTargets(), combatData, fightData, playerAgents, true);
        }
        if (!fightData.Success && (GenericFallBackMethod & FallBackMethod.CombatExit) > 0)
        {
            SetSuccessByCombatExit(GetSuccessCheckTargets(), combatData, fightData, playerAgents);
        }
    }

    internal virtual long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericFightOffset(fightData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            startToUse = GetEnterCombatTime(fightData, agentData, combatData, logStartNPCUpdate.Time, GenericTriggerID, logStartNPCUpdate.DstAgent);
        }
        return startToUse;
    }

    internal virtual FightLogic AdjustLogic(AgentData agentData, List<CombatItem> combatData)
    {
        return this;
    }

    internal virtual void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        ComputeFightTargets(agentData, combatData, extensions);
    }

    /// <summary>
    /// The buff must be present on any player at the end of the encounter.<br></br>
    /// </summary>
    /// <param name="stack">Amount of buff stacks (0-99).</param>
    /// <returns>
    /// A pari of (<paramref name="buff"/> and its <paramref name="stack"/>) if present, otherwise null.<br></br>
    /// To be used to add to <see cref="InstanceBuffs"/>. Use <see cref="ListExt.MaybeAdd"/>.
    /// </returns>
    protected static (Buff, int)? GetOnPlayerCustomInstanceBuff(ParsedEvtcLog log, long buff, int stack = 1)
    {
        foreach (Player p in log.PlayerList)
        {
            if (p.HasBuff(log, buff, log.FightData.FightEnd - ServerDelayConstant))
            {
                return (log.Buffs.BuffsByIds[buff], stack);
            }
        }
        return null;
    }
}
