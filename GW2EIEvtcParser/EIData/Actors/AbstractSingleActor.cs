using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

public abstract partial class AbstractSingleActor : AbstractActor
{
    public new AgentItem AgentItem => base.AgentItem;
    public string Account { get; protected set; }
    public int Group { get; protected set; }

    // Helpers
    public readonly EXTAbstractSingleActorHealingHelper EXTHealing;
    public readonly EXTAbstractSingleActorBarrierHelper EXTBarrier;
    // Minions
    private Dictionary<long, Minions>? _minions;
    // Replay
    private readonly Dictionary<DamageType, CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>> _typedSelfHitDamageEvents = [];
    protected CombatReplay CombatReplay;
    // Statistics
    private CachingCollectionWithTarget<FinalDPS>? _dpsStats;
    private CachingCollectionWithTarget<FinalDefenses>? _defenseStats;
    private CachingCollectionWithTarget<FinalOffensiveStats>? _offensiveStats;
    private CachingCollection<FinalGameplayStats>? _gameplayStats;
    private CachingCollectionWithTarget<FinalSupport>? _supportStats;
    private CachingCollection<FinalToPlayersSupport>? _toPlayerSupportStats;

    protected AbstractSingleActor(AgentItem agent) : base(agent)
    {
        Group = 51;
        Account = Character;
        EXTHealing = new EXTAbstractSingleActorHealingHelper(this);
        EXTBarrier = new EXTAbstractSingleActorBarrierHelper(this);
    }

    internal abstract void OverrideName(string name);

    public abstract string GetIcon();

    #region STATUS

    protected int Health = -2;

    public int GetHealth(CombatData combatData)
    {
        if (Health == -2)
        {
            Health = -1;
            IReadOnlyList<MaxHealthUpdateEvent> maxHpUpdates = combatData.GetMaxHealthUpdateEvents(AgentItem);
            if (maxHpUpdates.Any())
            {
                AbstractHealthDamageEvent lastDamage = combatData.GetDamageTakenData(AgentItem).LastOrDefault(x => x.HealthDamage > 0);
                long timeCheck = (FirstAware + LastAware) / 2;
                if (lastDamage != null)
                {
                    timeCheck = Math.Max(timeCheck, Math.Max(FirstAware, lastDamage.Time - 5000));
                }
                MaxHealthUpdateEvent hpEvent = maxHpUpdates.LastOrDefault(x => x.Time < timeCheck + ServerDelayConstant);
                Health = hpEvent != null ? hpEvent.MaxHealth : maxHpUpdates.Max(x => x.MaxHealth);
            }
        }
        return Health;
    }

    internal abstract void SetManualHealth(int health, IReadOnlyList<(long hpValue, double percent)>? hpDistribution = null);

    public virtual IReadOnlyList<(long hpValue, double percent)>? GetHealthDistribution()
    {
        return null;
    }


    public bool IsDowned(ParsedEvtcLog log, long time)
    {
        (_, IReadOnlyList<Segment> downs, _) = GetStatus(log);
        return downs.Any(x => x.ContainsPoint(time));
    }
    public bool IsDowned(ParsedEvtcLog log, long start, long end)
    {
        (_, IReadOnlyList<Segment> downs, _) = GetStatus(log);
        return downs.Any(x => x.Intersects(start, end));
    }
    public bool IsDead(ParsedEvtcLog log, long time)
    {
        (IReadOnlyList<Segment> deads, _, _) = GetStatus(log);
        return deads.Any(x => x.ContainsPoint(time));
    }
    public bool IsDead(ParsedEvtcLog log, long start, long end)
    {
        (IReadOnlyList<Segment> deads, _, _) = GetStatus(log);
        return deads.Any(x => x.Intersects(start, end));
    }
    public bool IsDC(ParsedEvtcLog log, long time)
    {
        (_, _, IReadOnlyList<Segment> dcs) = GetStatus(log);
        return dcs.Any(x => x.ContainsPoint(time));
    }
    public bool IsDC(ParsedEvtcLog log, long start, long end)
    {
        (_, _, IReadOnlyList<Segment> dcs) = GetStatus(log);
        return dcs.Any(x => x.Intersects(start, end));
    }

    public ArcDPSEnums.BreakbarState GetCurrentBreakbarState(ParsedEvtcLog log, long time)
    {
        var (nones, actives, immunes, recoverings) = GetBreakbarStatus(log);
        if (nones.Any(x => x.ContainsPoint(time)))
        {
            return ArcDPSEnums.BreakbarState.None;
        }

        if (actives.Any(x => x.ContainsPoint(time)))
        {
            return ArcDPSEnums.BreakbarState.Active;
        }

        if (immunes.Any(x => x.ContainsPoint(time)))
        {
            return ArcDPSEnums.BreakbarState.Immune;
        }

        if (recoverings.Any(x => x.ContainsPoint(time)))
        {
            return ArcDPSEnums.BreakbarState.Recover;
        }

        return ArcDPSEnums.BreakbarState.None;
    }


    /// <summary>
    /// Return the health value at requested %
    /// </summary>
    public abstract int GetCurrentHealth(ParsedEvtcLog log, double currentHealthPercent);



    /// <summary>
    /// Return the health value at requested time
    /// </summary>
    public int GetCurrentHealth(ParsedEvtcLog log, long time)
    {
        var currentHPPercent = GetCurrentHealthPercent(log, time);
        return GetCurrentHealth(log, currentHPPercent);
    }

    /// <summary>
    /// Return the barrier value at requested %
    /// </summary>
    /// <param name="time">Time at which to check for barrier. Barrier scales of the current maximum health of the actor and maximum health can change dynamically</param>
    public abstract int GetCurrentBarrier(ParsedEvtcLog log, double currentBarrierPercent, long time);

    /// <summary>
    /// Return the barrier value at requested time
    /// </summary>
    /// <param name="time">Time at which to check for barrier. Barrier scales of the current maximum health of the actor and maximum health can change dynamically</param>
    public int GetCurrentBarrier(ParsedEvtcLog log, long time)
    {
        var currentBarrierPercent = GetCurrentBarrierPercent(log, time);
        return GetCurrentBarrier(log, currentBarrierPercent, time);
    }

    #endregion STATUS


    // Minions
    public IReadOnlyDictionary<long, Minions> GetMinions(ParsedEvtcLog log)
    {
        if (_minions == null)
        {
            _minions = [];
            // npcs, species id based
            var combatMinion = log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.Master != null && x.GetFinalMaster() == AgentItem).ToList();
            var auxMinions = new Dictionary<long, Minions>();
            foreach (AgentItem agent in combatMinion)
            {
                long id = agent.ID;
                var singleActor = log.FindActor(agent);
                if (singleActor is NPC npc)
                {
                    if (auxMinions.TryGetValue(id, out Minions values))
                    {
                        values.AddMinion(npc);
                    }
                    else
                    {
                        auxMinions[id] = new Minions(this, npc);
                    }
                }
            }
            foreach (KeyValuePair<long, Minions> pair in auxMinions)
            {
                if (pair.Value.IsActive(log))
                {
                    _minions[pair.Value.UniqueID] = pair.Value;
                }
            }
            // gadget, string based
            var combatGadgetMinion = log.AgentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => x.Master != null && x.GetFinalMaster() == AgentItem).ToList();
            var auxGadgetMinions = new Dictionary<string, Minions>();
            foreach (AgentItem agent in combatGadgetMinion)
            {
                string id = agent.Name;
                var singleActor = log.FindActor(agent);
                if (singleActor is NPC npc)
                {
                    if (auxGadgetMinions.TryGetValue(id, out Minions values))
                    {
                        values.AddMinion(npc);
                    }
                    else
                    {
                        auxGadgetMinions[id] = new Minions(this, npc);
                    }
                }
            }
            foreach (KeyValuePair<string, Minions> pair in auxGadgetMinions)
            {
                if (pair.Value.IsActive(log))
                {
                    _minions[pair.Value.UniqueID] = pair.Value;
                }
            }
        }
        return _minions;
    }

    #region BUFFS

    internal virtual (Dictionary<long, FinalActorBuffs> Buffs, Dictionary<long, FinalActorBuffs> ActiveBuffs) ComputeBuffs(ParsedEvtcLog log, long start, long end, BuffEnum type)
    {
        return (type) switch
        {
            BuffEnum.Group or BuffEnum.OffGroup => ([ ], [ ]),
            BuffEnum.Squad =>
                FinalActorBuffs.GetBuffsForPlayers(log.PlayerList.Where(p => p != this), log, AgentItem, start, end),
            _ => FinalActorBuffs.GetBuffsForSelf(log, this, start, end),
        };
    }

    internal virtual (Dictionary<long, FinalActorBuffVolumes> Volumes, Dictionary<long, FinalActorBuffVolumes> ActiveVolumes) ComputeBuffVolumes(ParsedEvtcLog log, long start, long end, BuffEnum type)
    {
        return (type) switch 
        {
            BuffEnum.Group or BuffEnum.OffGroup => ([ ], [ ]),
            BuffEnum.Squad => 
                FinalActorBuffVolumes.GetBuffVolumesForPlayers(log.PlayerList.Where(p => p != this), log, AgentItem, start, end),
            _ => FinalActorBuffVolumes.GetBuffVolumesForSelf(log, this, start, end),
        };
    }

    /// <summary>
    /// Creates a <see cref="List{T}"/> of <see cref="Segment"/> of the <paramref name="buffIds"/> in input.
    /// </summary>
    /// <param name="buffIds">Buff IDs of which to find the <see cref="Segment"/> of.</param>
    /// <param name="start">Start time to search.</param>
    /// <param name="end">End time to search.</param>
    /// <returns><see cref="List{T}"/> with the <see cref="Segment"/>s found.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public List<Segment> GetBuffStatus(ParsedEvtcLog log, long[] buffIds, long start, long end)
    {
        //TODO(Rennorb) @perf
        var result = new List<Segment>();
        foreach (long id in buffIds)
        {
            result.AddRange(GetBuffStatus(log, id, start, end));
        }
        return result;
    }

    #endregion BUFFS

    #region COMBAT REPLAY
    protected void SetMovements(ParsedEvtcLog log)
    {
        foreach (AbstractMovementEvent movementEvent in log.CombatData.GetMovementData(AgentItem))
        {
            movementEvent.AddPoint3D(CombatReplay);
        }
    }

    public bool HasPositions(ParsedEvtcLog log)
    {
        return GetCombatReplayNonPolledPositions(log).Count > 0;
    }

    public bool HasCombatReplayPositions(ParsedEvtcLog log)
    {
        return HasPositions(log) && GetCombatReplayPolledPositions(log).Count > 0;
    }


    public bool HasRotations(ParsedEvtcLog log)
    {
        return GetCombatReplayNonPolledRotations(log).Count > 0;
    }

    public bool HasCombatReplayRotations(ParsedEvtcLog log)
    {
        return HasRotations(log) && GetCombatReplayPolledRotations(log).Count > 0;
    }

    public IReadOnlyList<ParametricPoint3D> GetCombatReplayNonPolledPositions(ParsedEvtcLog log)
    {
        if (CombatReplay == null)
        {
            InitCombatReplay(log);
        }
        return CombatReplay.Positions;
    }

    public IReadOnlyList<ParametricPoint3D> GetCombatReplayPolledPositions(ParsedEvtcLog log)
    {
        if (CombatReplay == null)
        {
            InitCombatReplay(log);
        }
        return CombatReplay.PolledPositions;
    }

    public IReadOnlyList<ParametricPoint3D> GetCombatReplayNonPolledRotations(ParsedEvtcLog log)
    {
        if (CombatReplay == null)
        {
            InitCombatReplay(log);
        }
        return CombatReplay.Rotations;
    }

    public IReadOnlyList<ParametricPoint3D> GetCombatReplayPolledRotations(ParsedEvtcLog log)
    {
        if (CombatReplay == null)
        {
            InitCombatReplay(log);
        }
        return CombatReplay.PolledRotations;
    }

    protected static void TrimCombatReplay(ParsedEvtcLog log, CombatReplay replay, AgentItem agentItem)
    {
        // Trim
        DespawnEvent despawnCheck = log.CombatData.GetDespawnEvents(agentItem).LastOrDefault();
        SpawnEvent spawnCheck = log.CombatData.GetSpawnEvents(agentItem).LastOrDefault();
        DeadEvent deathCheck = log.CombatData.GetDeadEvents(agentItem).LastOrDefault();
        AliveEvent aliveCheck = log.CombatData.GetAliveEvents(agentItem).LastOrDefault();
        if (deathCheck != null && (aliveCheck == null || aliveCheck.Time < deathCheck.Time))
        {
            replay.Trim(agentItem.FirstAware, deathCheck.Time);
        }
        else if (despawnCheck != null && (spawnCheck == null || spawnCheck.Time < despawnCheck.Time))
        {
            replay.Trim(agentItem.FirstAware, despawnCheck.Time);
        }
        else
        {
            replay.Trim(agentItem.FirstAware, agentItem.LastAware);
        }
    }

    protected virtual void TrimCombatReplay(ParsedEvtcLog log)
    {

    }

    [MemberNotNull(nameof(CombatReplay))]
    protected void InitCombatReplay(ParsedEvtcLog log)
    {
        CombatReplay = new CombatReplay(log);
        if (!log.CombatData.HasMovementData)
        {
            // no combat replay support on fight
            return;
        }
        SetMovements(log);
        CombatReplay.PollingRate(log.FightData.FightDuration, AgentItem.Type == AgentItem.AgentType.Player);
        TrimCombatReplay(log);
        if (!IsFakeActor)
        {
            InitAdditionalCombatReplayData(log);
        }
    }

    internal IReadOnlyList<GenericDecorationRenderingDescription> GetCombatReplayDecorationRenderableDescriptions(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {

        if (CombatReplay == null)
        {
            InitCombatReplay(log);
        }
        return CombatReplay.Decorations.GetCombatReplayRenderableDescriptions(map, log, usedSkills, usedBuffs);
    }

    protected virtual void InitAdditionalCombatReplayData(ParsedEvtcLog log)
    {
        foreach (var squadMarker in MarkerGUIDs.SquadOverheadMarkersHexGUIDs)
        {
            if (log.CombatData.TryGetMarkerEventsBySrcWithGUID(AgentItem, squadMarker, out var markerEvents))
            {
                foreach (MarkerEvent markerEvent in markerEvents)
                {
                    if (ParserIcons.SquadMarkerToIcon.TryGetValue(squadMarker, out string icon))
                    {
                        CombatReplay.AddRotatedOverheadMarkerIcon(new Segment(markerEvent.Time, markerEvent.EndTime, 1), this, icon, 240f, 16, 1);
                    }
                }
            }
        }
    }

    public abstract AbstractSingleActorCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log);

    private static bool TryGetCurrentPoint(IReadOnlyList<ParametricPoint3D> points, long time, [NotNullWhen(true)] out Vector3 point, long forwardWindow = 0)
    {
        if (forwardWindow != 0)
        {
           var parametric = points.FirstOrNull((in ParametricPoint3D x) => x.Time >= time && x.Time <= time + forwardWindow)
                ?? points.LastOrNull((in ParametricPoint3D x) => x.Time <= time);
           if(parametric.HasValue)
           {
                point = parametric.Value.Value;
                return true;
           }

            point = default;
            return false;
        }

        int foundIndex = BinarySearchRecursive(points, time, 0, points.Count - 1);
        if (foundIndex < 0)
        {
            point = default;
            return false;
        }
        
        ParametricPoint3D position = points[foundIndex];
        if (position.Time > time)
        {
            point = default;
            return false;
        }
        
        point = position.Value;
        return true;
    }

    /// <param name="forwardWindow">Position will be looked up to time + forwardWindow if given</param>
    public bool TryGetCurrentPosition(ParsedEvtcLog log, long time, [NotNullWhen(true)] out Vector3 position, long forwardWindow = 0)
    {
        if (!HasCombatReplayPositions(log))
        {
            if (HasPositions(log))
            {
                return TryGetCurrentPoint(GetCombatReplayNonPolledPositions(log), time, out position, forwardWindow);
            }

            position = default;
            return false;
        }
        return TryGetCurrentPoint(GetCombatReplayPolledPositions(log), time, out position, forwardWindow);
    }

    public bool TryGetCurrentInterpolatedPosition(ParsedEvtcLog log, long time, [NotNullWhen(true)] out Vector3 position)
    {
        if (!HasCombatReplayPositions(log))
        {
            position = default;
            return false;
        }

        IReadOnlyList<ParametricPoint3D> positions = GetCombatReplayPolledPositions(log);
        var next = positions.FirstOrNull((in ParametricPoint3D x) => x.Time >= time);
        var prev = positions.LastOrNull((in ParametricPoint3D x) => x.Time <= time);
        if (prev.HasValue && next.HasValue)
        {
            long denom = next.Value.Time - prev.Value.Time;
            if (denom == 0)
            {
                position = prev.Value.Value;
            }
            else
            {
                float ratio = (float)(time - prev.Value.Time) / denom;
                position = Vector3.Lerp(prev.Value.Value, next.Value.Value, ratio);
            }
            return true;
        }
        else
        {
            var parametric = prev ?? next;
            if(parametric != null)
            {
                position = parametric.Value.Value;
                return true;
            }
            else
            {
                position = default;
                return false;
            }
        }
    }

    //TODO(Rennorb) @cleanup: There is an argument to be made here that in all the places where this is used you probably want to add a decoration either way, regardless if this fails or not.
    //Something to look into.
    /// <param name="forwardWindow">Rotation will be looked up to time + forwardWindow if given</param>
    public bool TryGetCurrentFacingDirection(ParsedEvtcLog log, long time, [NotNullWhen(true)] out Vector3 rotation, long forwardWindow = 0) 
    {
        if (!HasCombatReplayRotations(log))
        {
            if (HasRotations(log))
            {
                return TryGetCurrentPoint(GetCombatReplayNonPolledRotations(log), time, out rotation, forwardWindow);
            }

            rotation = default;
            return false;
        }

        return TryGetCurrentPoint(GetCombatReplayPolledRotations(log), time, out rotation, forwardWindow);
    }

    #endregion COMBAT REPLAY

    #region CAST
    public override IEnumerable<AbstractCastEvent> GetCastEvents(ParsedEvtcLog log, long start, long end)
    {
        if (CastEvents == null)
        {
            SetCastEvents(log);
        }
        return CastEvents.Where(x => x.Time >= start && x.Time <= end);

    }
    public override IEnumerable<AbstractCastEvent> GetIntersectingCastEvents(ParsedEvtcLog log, long start, long end)
    {
        if (CastEvents == null)
        {
            SetCastEvents(log);
        }
        return CastEvents.Where(x => KeepIntersectingCastLog(x, start, end));

    }
    protected void SetCastEvents(ParsedEvtcLog log)
    {
        var animationCastData = log.CombatData.GetAnimatedCastData(AgentItem);
        var instantCastData = log.CombatData.GetInstantCastData(AgentItem);
        CastEvents = new List<AbstractCastEvent>(animationCastData.Count + instantCastData.Count);
        #pragma warning disable IDE0028 //NOTE(Rennorb): this is (liikely) mroe efficient because of the list tpes
        CastEvents.AddRange(animationCastData);
        CastEvents.AddRange(instantCastData);
        #pragma warning restore IDE0028 
        foreach (WeaponSwapEvent wepSwap in log.CombatData.GetWeaponSwapData(AgentItem))
        {
            if (CastEvents.Count > 0 && (wepSwap.Time - CastEvents.Last().Time) < ServerDelayConstant && CastEvents.Last().SkillId == WeaponSwap)
            {
                CastEvents[^1] = wepSwap;
            }
            else
            {
                CastEvents.Add(wepSwap);
            }
        }
        CastEvents.SortByTimeThenNegatedSwap();
    }
    #endregion CAST

    #region STATISTICS

    public FinalDPS GetDPSStats(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _dpsStats ??= new CachingCollectionWithTarget<FinalDPS>(log);

        if (!_dpsStats.TryGetValue(start, end, target, out var value))
        {
            value = new FinalDPS(log, start, end, this, target);
            _dpsStats.Set(start, end, target, value);
        }
        return value;
    }

    public FinalDPS GetDPSStats(ParsedEvtcLog log, long start, long end)
    {
        return GetDPSStats(null, log, start, end);
    }

    // Defense Stats

    public FinalDefenses GetDefenseStats(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _defenseStats ??= new CachingCollectionWithTarget<FinalDefenses>(log);

        if (!_defenseStats.TryGetValue(start, end, target, out var value))
        {
            value = target != null ? new FinalDefenses(log, start, end, this, target) : new FinalDefensesAll(log, start, end, this);
            _defenseStats.Set(start, end, target, value);
        }
        return value;
    }

    public FinalDefensesAll GetDefenseStats(ParsedEvtcLog log, long start, long end)
    {
        return (GetDefenseStats(null, log, start, end) as FinalDefensesAll)!;
    }

    // Gameplay Stats

    public FinalGameplayStats GetGameplayStats(ParsedEvtcLog log, long start, long end)
    {
        _gameplayStats ??= new(log, 4, 2); //TODO(Rennorb) @perf: find capacity dependencies

        if (!_gameplayStats.TryGetValue(start, end, out var value))
        {
            value = new FinalGameplayStats(log, start, end, this);
            _gameplayStats.Set(start, end, value);
        }
        return value;
    }

    public FinalOffensiveStats GetOffensiveStats(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _offensiveStats ??= new CachingCollectionWithTarget<FinalOffensiveStats>(log);

        if (!_offensiveStats.TryGetValue(start, end, target, out FinalOffensiveStats? value))
        {
            value = new FinalOffensiveStats(log, start, end, this, target);
            _offensiveStats.Set(start, end, target, value);
        }
        return value;
    }

    // Support stats
    public FinalSupportAll GetSupportStats(ParsedEvtcLog log, long start, long end)
    {
        return (GetSupportStats(null, log, start, end) as FinalSupportAll)!;
    }

    public FinalSupport GetSupportStats(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _supportStats ??= new CachingCollectionWithTarget<FinalSupport>(log);

        if (!_supportStats.TryGetValue(start, end, target, out FinalSupport? value))
        {
            value = target != null ? new FinalSupport(log, start, end, this, target) : new FinalSupportAll(log, start, end, this);
            _supportStats.Set(start, end, target, value);
        }
        return value;
    }

    public FinalToPlayersSupport GetToPlayerSupportStats(ParsedEvtcLog log, long start, long end)
    {
        _toPlayerSupportStats ??= new(log, 4, 2); //TODO(Rennorb) @perf: find capacity dependencies

        if (!_toPlayerSupportStats.TryGetValue(start, end, out var value))
        {
            value = new FinalToPlayersSupport(log, this, start, end);
            _toPlayerSupportStats.Set(start, end, value);
        }
        return value;
    }
    #endregion STATISTICS

    #region DAMAGE
    public override IEnumerable<AbstractHealthDamageEvent> GetDamageEvents(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (DamageEvents == null || DamageEventByDst == null)
        {
            DamageEvents = new List<AbstractHealthDamageEvent>(log.CombatData.GetDamageData(AgentItem).Where(x => !x.ToFriendly));
            IReadOnlyDictionary<long, Minions> minionsList = GetMinions(log); //TODO(Rennorb @perf: find average complexity
            foreach (Minions mins in minionsList.Values)
            {
                DamageEvents.AddRange(mins.GetDamageEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd));
            }
            DamageEvents.SortByTime();
            DamageEventByDst = DamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        }

        if (target != null)
        {
            if (DamageEventByDst.TryGetValue(target.AgentItem, out List<AbstractHealthDamageEvent> list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end);
            }
            else
            {
                return [ ];
            }
        }

        return DamageEvents.Where(x => x.Time >= start && x.Time <= end);
    }

    public IEnumerable<AbstractHealthDamageEvent> GetJustActorDamageEvents(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        return GetDamageEvents(target, log, start, end).Where(x => x.From == AgentItem);
    }

    public override IEnumerable<AbstractHealthDamageEvent> GetDamageTakenEvents(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (DamageTakenEvents == null)
        {
            DamageTakenEvents = new List<AbstractHealthDamageEvent>(log.CombatData.GetDamageTakenData(AgentItem));
            DamageTakenEvents.SortByTime();
            DamageTakenEventsBySrc = DamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        }

        if (target != null)
        {
            if (DamageTakenEventsBySrc!.TryGetValue(target.AgentItem, out List<AbstractHealthDamageEvent> list))
            {
                long targetStart = target.FirstAware;
                long targetEnd = target.LastAware;
                return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd);
            }
            else
            {
                return [ ];
            }
        }
        return DamageTakenEvents.Where(x => x.Time >= start && x.Time <= end);
    }

    /// <summary>
    /// cached method for damage modifiers
    /// </summary>
    internal IReadOnlyList<AbstractHealthDamageEvent> GetJustActorHitDamageEvents(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end, DamageType damageType)
    {
        if (!_typedSelfHitDamageEvents.TryGetValue(damageType, out CachingCollectionWithTarget<List<AbstractHealthDamageEvent>> hitDamageEventsPerPhasePerTarget))
        {
            hitDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>(log);
            _typedSelfHitDamageEvents[damageType] = hitDamageEventsPerPhasePerTarget;
        }
        if (!hitDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent>? dls))
        {
            dls = GetHitDamageEvents(target, log, start, end, damageType).Where(x => x.From == AgentItem).ToList();
            hitDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
        }
        return dls;
    }

    #endregion DAMAGE

    #region BREAKBAR DAMAGE

    public IEnumerable<BreakbarDamageEvent> GetJustActorBreakbarDamageEvents(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        return GetBreakbarDamageEvents(target, log, start, end).Where(x => x.From == AgentItem);
    }

    public override IEnumerable<BreakbarDamageEvent> GetBreakbarDamageEvents(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (BreakbarDamageEvents == null)
        {
            BreakbarDamageEvents = new List<BreakbarDamageEvent>(log.CombatData.GetBreakbarDamageData(AgentItem).Where(x => !x.ToFriendly));
            IReadOnlyDictionary<long, Minions> minionsList = GetMinions(log); //TODO(Rennorb) @perf: find average complexity
            foreach (Minions mins in minionsList.Values)
            {
                BreakbarDamageEvents.AddRange(mins.GetBreakbarDamageEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd));
            }
            BreakbarDamageEvents.SortByTime();
            BreakbarDamageEventsByDst = BreakbarDamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        }

        if (target != null)
        {
            if (BreakbarDamageEventsByDst!.TryGetValue(target.AgentItem, out var list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end);
            }
            else
            {
                return [ ];
            }
        }

        return BreakbarDamageEvents.Where(x => x.Time >= start && x.Time <= end);
    }

    public override IEnumerable<BreakbarDamageEvent> GetBreakbarDamageTakenEvents(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (BreakbarDamageTakenEvents == null)
        {
            BreakbarDamageTakenEvents = new List<BreakbarDamageEvent>(log.CombatData.GetBreakbarDamageTakenData(AgentItem));
            BreakbarDamageTakenEvents.SortByTime();
            BreakbarDamageTakenEventsBySrc = BreakbarDamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        }

        if (target != null)
        {
            if (BreakbarDamageTakenEventsBySrc!.TryGetValue(target.AgentItem, out var list))
            {
                long targetStart = target.FirstAware;
                long targetEnd = target.LastAware;
                return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd);
            }
            else
            {
                return [ ];
            }
        }

        return BreakbarDamageTakenEvents.Where(x => x.Time >= start && x.Time <= end);
    }

    #endregion BREAKBAR DAMAGE

    #region CROWD CONTROL

    public IEnumerable<CrowdControlEvent> GetJustOutgoingActorCrowdControlEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
    {
        return GetOutgoingCrowdControlEvents(target, log, start, end).Where(x => x.From == AgentItem);
    }

    public override IEnumerable<CrowdControlEvent> GetOutgoingCrowdControlEvents(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (OutgoingCrowdControlEvents == null)
        {
            OutgoingCrowdControlEvents = new List<CrowdControlEvent>(log.CombatData.GetOutgoingCrowdControlData(AgentItem).Where(x => !x.ToFriendly));
            IReadOnlyDictionary<long, Minions> minionsList = GetMinions(log);
            foreach (Minions mins in minionsList.Values)
            {
                OutgoingCrowdControlEvents.AddRange(mins.GetOutgoingCrowdControlEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd));
            }
            OutgoingCrowdControlEvents.SortByTime();
            OutgoingCrowdControlEventsByDst = OutgoingCrowdControlEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        }

        if (target != null)
        {
            if (OutgoingCrowdControlEventsByDst!.TryGetValue(target.AgentItem, out List<CrowdControlEvent> list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end);
            }
            else
            {
                return [ ];
            }
        }

        return OutgoingCrowdControlEvents.Where(x => x.Time >= start && x.Time <= end);
    }

    public override IEnumerable<CrowdControlEvent> GetIncomingCrowdControlEvents(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (IncomingCrowdControlEvents == null)
        {
            IncomingCrowdControlEvents = new List<CrowdControlEvent>(log.CombatData.GetIncomingCrowdControlData(AgentItem));
            IncomingCrowdControlEvents.SortByTime();
            IncomingCrowdControlEventsBySrc = IncomingCrowdControlEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        }

        if (target != null)
        {
            if (IncomingCrowdControlEventsBySrc!.TryGetValue(target.AgentItem, out var list))
            {
                long targetStart = target.FirstAware;
                long targetEnd = target.LastAware;
                return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd);
            }
            else
            {
                return [ ];
            }
        }

        return IncomingCrowdControlEvents.Where(x => x.Time >= start && x.Time <= end);
    }

    #endregion CROWD CONTROL


    // https://www.c-sharpcorner.com/blogs/binary-search-implementation-using-c-sharp1
    private static int BinarySearchRecursive(IReadOnlyList<ParametricPoint3D> position, long time, int minIndex, int maxIndex)
    {
        if (position.Count == 0)
        {
            return -1;
        }

        if (position[minIndex].Time > time)
        {
            return minIndex - 1;
        }

        if (position[maxIndex].Time < time)
        {
            return maxIndex;
        }

        if (minIndex > maxIndex)
        {
            return minIndex - 1;
        }
        else
        {
            int midIndex = (minIndex + maxIndex) / 2;
            if (time == position[midIndex].Time)
            {
                return midIndex;
            }
            else if (time < position[midIndex].Time)
            {
                return BinarySearchRecursive(position, time, minIndex, midIndex - 1);
            }
            else
            {
                return BinarySearchRecursive(position, time, midIndex + 1, maxIndex);
            }
        }
    }
}
