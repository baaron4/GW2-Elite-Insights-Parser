using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

public abstract partial class SingleActor : Actor
{
    public new AgentItem AgentItem => base.AgentItem;

    public AgentItem EnglobingAgentItem => AgentItem.EnglobingAgentItem;
    public string Account { get; protected set; }
    public int Group { get; protected set; }

    // Helpers
    public readonly EXTSingleActorHealingHelper EXTHealing;
    public readonly EXTSingleActorBarrierHelper EXTBarrier;

    protected SingleActor(AgentItem agent) : base(agent)
    {
        Group = 51;
        Account = Character;
        EXTHealing = new EXTSingleActorHealingHelper(this);
        EXTBarrier = new EXTSingleActorBarrierHelper(this);
    }

    internal abstract void OverrideName(string name);

    public abstract string GetIcon(bool forceLowResolutionIfApplicable = false);

    #region STATUS

    protected int Health = -2;

    public int GetHealth(CombatData combatData)
    {
        if (Health == -2)
        {
            Health = -1;
            IReadOnlyList<MaxHealthUpdateEvent> maxHpUpdates = combatData.GetMaxHealthUpdateEvents(EnglobingAgentItem);
            if (maxHpUpdates.Any())
            {
                HealthDamageEvent? lastDamage = combatData.GetDamageTakenData(AgentItem).LastOrDefault(x => x.HealthDamage > 0);
                long timeCheck = (FirstAware + LastAware) / 2;
                if (lastDamage != null)
                {
                    timeCheck = Math.Max(timeCheck, Math.Max(FirstAware, lastDamage.Time - 5000));
                }
                MaxHealthUpdateEvent? hpEvent = maxHpUpdates.LastOrDefault(x => x.Time < timeCheck + ServerDelayConstant);
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
    private Dictionary<long, Minions>? _minions;
    public IReadOnlyDictionary<long, Minions> GetMinions(ParsedEvtcLog log)
    {
        if (_minions == null)
        {
            _minions = [];
            // npcs, species id based
            var combatMinion = log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => AgentItem.IsMasterOf(x));
            var auxMinions = new Dictionary<long, Minions>();
            foreach (AgentItem agent in combatMinion)
            {
                if (!agent.InAwareTimes(AgentItem))
                {
                    continue;
                }
                long id = agent.ID;
                var singleActor = log.FindActor(agent);
                if (singleActor is NPC npc)
                {
                    if (auxMinions.TryGetValue(id, out var values))
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
            var combatGadgetMinion = log.AgentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => AgentItem.IsMasterOf(x));
            var auxGadgetMinions = new Dictionary<string, Minions>();
            foreach (AgentItem agent in combatGadgetMinion)
            {
                if (!agent.InAwareTimes(AgentItem))
                {
                    continue;
                }
                string id = agent.Name;
                var singleActor = log.FindActor(agent);
                if (singleActor is NPC npc)
                {
                    if (auxGadgetMinions.TryGetValue(id, out var values))
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

    internal virtual (Dictionary<long, BuffStatistics> Buffs, Dictionary<long, BuffStatistics> ActiveBuffs) ComputeBuffs(ParsedEvtcLog log, long start, long end, BuffEnum type)
    {
        return (type) switch
        {
            BuffEnum.Group or BuffEnum.OffGroup => ([ ], [ ]),
            BuffEnum.Squad =>
                BuffStatistics.GetBuffsForPlayers(log.PlayerList.Where(p => p != this), log, AgentItem, start, end),
            _ => BuffStatistics.GetBuffsForSelf(log, this, start, end),
        };
    }

    internal virtual (Dictionary<long, BuffVolumeStatistics> Volumes, Dictionary<long, BuffVolumeStatistics> ActiveVolumes) ComputeBuffVolumes(ParsedEvtcLog log, long start, long end, BuffEnum type)
    {
        return (type) switch 
        {
            BuffEnum.Group or BuffEnum.OffGroup => ([ ], [ ]),
            BuffEnum.Squad => 
                BuffVolumeStatistics.GetBuffVolumesForPlayers(log.PlayerList.Where(p => p != this), log, this, start, end),
            _ => BuffVolumeStatistics.GetBuffVolumesForSelf(log, this, start, end),
        };
    }

    #endregion BUFFS

    #region COMBAT REPLAY

    private CombatReplay? CombatReplay;
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
        return InitCombatReplay(log).Positions;
    }

    public IReadOnlyList<ParametricPoint3D> GetCombatReplayPolledPositions(ParsedEvtcLog log)
    {
        return InitCombatReplay(log).PolledPositions;
    }

    /// <summary> Calculates a list of positions of the player which are null in places where the player is dead or disconnected. </summary>
    public List<ParametricPoint3D?> GetCombatReplayActivePolledPositions(ParsedEvtcLog log)
    {
        var (_, _, _, actives) = GetStatus(log);
        var positions = GetCombatReplayPolledPositions(log);
        var activePositions = new List<ParametricPoint3D?>(positions.Count);
        bool canCR = HasCombatReplayPositions(log);
        int positionIndex = 0;
        for (int j = 0; j < actives.Count; j++)
        {
            var active = actives[j];
            for (; positionIndex < positions.Count; positionIndex++)
            {
                var cur = positions[positionIndex];
                if (!canCR)
                {
                    activePositions.Add(null);
                } 
                else if (active.End < cur.Time)
                {
                    break;
                }
                else if (active.Start > cur.Time)
                {
                    activePositions.Add(null);
                }
                else
                {
                    activePositions.Add(cur);
                }
            }
        }
        return activePositions;
    }

    public IReadOnlyList<ParametricPoint3D> GetCombatReplayNonPolledRotations(ParsedEvtcLog log)
    {
        return InitCombatReplay(log).Rotations;
    }

    public IReadOnlyList<ParametricPoint3D> GetCombatReplayPolledRotations(ParsedEvtcLog log)
    {
        return InitCombatReplay(log).PolledRotations;
    }

    protected virtual IReadOnlyList<Segment> GetActiveSegmentsForCRTrim(ParsedEvtcLog log)
    {
        var (_, _, _, actives) = GetStatus(log);
        return actives;
    }
    private void TrimCombatReplay(ParsedEvtcLog log, CombatReplay replay)
    {
        var actives = GetActiveSegmentsForCRTrim(log);
        long trimStart = FirstAware;
        long trimEnd = LastAware;
        long previousActiveEnd = FirstAware;
        for (int i = 0; i < actives.Count - 1; i++)
        {
            if (i == 0)
            {
                trimStart = actives[i].Start;
            }
            else if (previousActiveEnd != actives[i].Start)
            {
                replay.Hidden.Add(new Segment(previousActiveEnd, actives[i].Start));
            }
            previousActiveEnd = actives[i].End;
        }
        if (actives.Count > 0)
        {
            var last = actives[actives.Count - 1];
            if (actives.Count == 1)
            {
                trimStart = actives[0].Start;
            }
            else if (previousActiveEnd != last.Start)
            {
                replay.Hidden.Add(new Segment(previousActiveEnd, last.Start));
            }
            trimEnd = last.End;
        }
        replay.Trim(Math.Max(trimStart, FirstAware), Math.Min(trimEnd, LastAware));
    }
    
    [MemberNotNull(nameof(CombatReplay))]
    protected CombatReplay InitCombatReplay(ParsedEvtcLog log)
    {
        if (CombatReplay != null)
        {
            return CombatReplay;
        }
        CombatReplay = new CombatReplay(log);
        if (!log.CombatData.HasMovementData)
        {
            // no combat replay support on log
            return CombatReplay;
        }
        if (AgentItem.IsEnglobedAgent)
        {
            // Use position data from englobing
            var parentActor = log.FindActor(AgentItem.EnglobingAgentItem);
            CombatReplay.Positions.AddRange(parentActor.GetCombatReplayNonPolledPositions(log));
            CombatReplay.PolledPositions.AddRange(parentActor.GetCombatReplayPolledPositions(log));
            CombatReplay.Rotations.AddRange(parentActor.GetCombatReplayNonPolledRotations(log));
            CombatReplay.PolledRotations.AddRange(parentActor.GetCombatReplayPolledRotations(log));
        } 
        else
        {
            foreach (MovementEvent movementEvent in log.CombatData.GetMovementData(AgentItem))
            {
                movementEvent.AddPoint3D(CombatReplay);
            }
            CombatReplay.PollingRate(log.LogData.LogDuration, AgentItem.Type == AgentItem.AgentType.Player);
        }
        TrimCombatReplay(log, CombatReplay);
        if (!IsFakeActor && !AgentItem.IsEnglobingAgent)
        {
            InitAdditionalCombatReplayData(log, CombatReplay);
        }
        return CombatReplay;
    }

    internal IReadOnlyList<DecorationRenderingDescription> GetCombatReplayDecorationRenderableDescriptions(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        return InitCombatReplay(log).Decorations.GetCombatReplayRenderableDescriptions(map, log, usedSkills, usedBuffs);
    }

    protected virtual void InitAdditionalCombatReplayData(ParsedEvtcLog log, CombatReplay replay)
    {
        foreach (var squadMarker in MarkerGUIDs.SquadOverheadMarkersHexGUIDs)
        {
            if (log.CombatData.TryGetMarkerEventsBySrcWithGUID(AgentItem, squadMarker, out var markerEvents))
            {
                foreach (MarkerEvent markerEvent in markerEvents)
                {
                    if (ParserIcons.SquadMarkerToIcon.TryGetValue(squadMarker, out var icon))
                    {
                        replay.Decorations.AddRotatedOverheadMarkerIcon(new Segment(markerEvent.Time, markerEvent.EndTime, 1), this, icon, 240f, 16, 1);
                    }
                }
            }
        }
    }

    public abstract SingleActorCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log);

    private static bool TryGetCurrentPoint(IReadOnlyList<ParametricPoint3D> points, long time, [NotNullWhen(true)] out Vector3 point, long forwardWindow = 0)
    {
        if (forwardWindow != 0)
        {
           var parametric = points.FirstOrNull((in ParametricPoint3D x) => x.Time >= time && x.Time <= time + forwardWindow)
                ?? points.LastOrNull((in ParametricPoint3D x) => x.Time <= time);
           if(parametric.HasValue)
           {
                point = parametric.Value.XYZ;
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
        
        point = position.XYZ;
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
                position = prev.Value.XYZ;
            }
            else
            {
                float ratio = (float)(time - prev.Value.Time) / denom;
                position = Vector3.Lerp(prev.Value.XYZ, next.Value.XYZ, ratio);
            }
            return true;
        }
        else
        {
            var parametric = prev ?? next;
            if(parametric != null)
            {
                position = parametric.Value.XYZ;
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

    private CachingCollection<List<AnimatedCastEvent>>? _animatedCastEventsCache;
    public IReadOnlyList<AnimatedCastEvent> GetAnimatedCastEvents(ParsedEvtcLog log, long start, long end)
    {
        _animatedCastEventsCache ??= new CachingCollection<List<AnimatedCastEvent>>(log);
        if (!_animatedCastEventsCache.TryGetValue(start, end, out var list))
        {
            list = log.CombatData.GetAnimatedCastData(AgentItem).Where(x => x.Time >= start && x.Time <= end).ToList();
            _animatedCastEventsCache.Set(start, end, list);
        }
        return list;
    }

    public IReadOnlyList<AnimatedCastEvent> GetAnimatedCastEvents(ParsedEvtcLog log)
    {
        return GetAnimatedCastEvents(log, log.LogData.LogStart, log.LogData.LogEnd);
    }

    private CachingCollection<List<InstantCastEvent>>? _instantCastEventsCache;
    public IReadOnlyList<InstantCastEvent> GetInstantCastEvents(ParsedEvtcLog log, long start, long end)
    {
        _instantCastEventsCache ??= new CachingCollection<List<InstantCastEvent>>(log);
        if (!_instantCastEventsCache.TryGetValue(start, end, out var list))
        {
            list = log.CombatData.GetInstantCastData(AgentItem).Where(x => x.Time >= start && x.Time <= end).ToList();
            _instantCastEventsCache.Set(start, end, list);
        }
        return list;
    }

    public IReadOnlyList<InstantCastEvent> GetInstantCastEvents(ParsedEvtcLog log)
    {
        return GetInstantCastEvents(log, log.LogData.LogStart, log.LogData.LogEnd);
    }

    protected override void InitCastEvents(ParsedEvtcLog log)
    {
        var animationCastData = log.CombatData.GetAnimatedCastData(AgentItem);
        var instantCastData = log.CombatData.GetInstantCastData(AgentItem);
        #pragma warning disable IDE0028 //NOTE(Rennorb): this is (likely) more efficient because of the list types
        CastEvents = new List<CastEvent>(animationCastData.Count + instantCastData.Count);
        CastEvents.AddRange(animationCastData);
        CastEvents.AddRange(instantCastData);
        #pragma warning restore IDE0028 
        foreach (WeaponSwapEvent wepSwap in log.CombatData.GetWeaponSwapData(AgentItem))
        {
            if (CastEvents.Count > 0 && (wepSwap.Time - CastEvents.Last().Time) < ServerDelayConstant && CastEvents.Last().SkillID == WeaponSwap)
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

    private CachingCollectionWithTarget<DamageStatistics>? _dpsStats;
    public DamageStatistics GetDamageStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _dpsStats ??= new CachingCollectionWithTarget<DamageStatistics>(log);

        if (!_dpsStats.TryGetValue(start, end, target, out var value))
        {
            value = new DamageStatistics(log, start, end, this, target);
            _dpsStats.Set(start, end, target, value);
        }
        return value;
    }

    public DamageStatistics GetDamageStats(ParsedEvtcLog log, long start, long end)
    {
        return GetDamageStats(null, log, start, end);
    }

    // Defense Stats

    private CachingCollectionWithTarget<DefensePerTargetStatistics>? _defenseStats;
    public DefensePerTargetStatistics GetDefenseStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _defenseStats ??= new CachingCollectionWithTarget<DefensePerTargetStatistics>(log);

        if (!_defenseStats.TryGetValue(start, end, target, out var value))
        {
            value = target != null ? new DefensePerTargetStatistics(log, start, end, this, target) : new DefenseAllStatistics(log, start, end, this);
            _defenseStats.Set(start, end, target, value);
        }
        return value;
    }

    public DefenseAllStatistics GetDefenseStats(ParsedEvtcLog log, long start, long end)
    {
        return (GetDefenseStats(null, log, start, end) as DefenseAllStatistics)!;
    }

    // Gameplay Stats

    private CachingCollection<GameplayStatistics>? _gameplayStats;
    public GameplayStatistics GetGameplayStats(ParsedEvtcLog log, long start, long end)
    {
        _gameplayStats ??= new(log); 

        if (!_gameplayStats.TryGetValue(start, end, out var value))
        {
            value = new GameplayStatistics(log, start, end, this);
            _gameplayStats.Set(start, end, value);
        }
        return value;
    }

    private CachingCollectionWithTarget<OffensiveStatistics>? _offensiveStats;
    public OffensiveStatistics GetOffensiveStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _offensiveStats ??= new CachingCollectionWithTarget<OffensiveStatistics>(log);

        if (!_offensiveStats.TryGetValue(start, end, target, out OffensiveStatistics? value))
        {
            value = new OffensiveStatistics(log, start, end, this, target);
            _offensiveStats.Set(start, end, target, value);
        }
        return value;
    }

    // Support stats
    public SupportAllStatistics GetSupportStats(ParsedEvtcLog log, long start, long end)
    {
        return (GetSupportStats(null, log, start, end) as SupportAllStatistics)!;
    }

    private CachingCollectionWithTarget<SupportPerAllyStatistics>? _supportStats;
    public SupportPerAllyStatistics GetSupportStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _supportStats ??= new CachingCollectionWithTarget<SupportPerAllyStatistics>(log);

        if (!_supportStats.TryGetValue(start, end, target, out SupportPerAllyStatistics? value))
        {
            value = target != null ? new SupportPerAllyStatistics(log, start, end, this, target) : new SupportAllStatistics(log, start, end, this);
            _supportStats.Set(start, end, target, value);
        }
        return value;
    }

    private CachingCollection<SupportStatistics>? _toPlayerSupportStats;
    public SupportStatistics GetToAllySupportStats(ParsedEvtcLog log, long start, long end)
    {
        _toPlayerSupportStats ??= new(log);

        if (!_toPlayerSupportStats.TryGetValue(start, end, out var value))
        {
            value = new SupportStatistics(log, this, start, end);
            _toPlayerSupportStats.Set(start, end, value);
        }
        return value;
    }
    #endregion STATISTICS

    #region DAMAGE

    protected override void InitDamageEvents(ParsedEvtcLog log)
    {
        if (DamageEvents == null || DamageEventByDst == null)
        {
            DamageEvents = [.. log.CombatData.GetDamageData(AgentItem).Where(x => !x.ToFriendly)];
            IReadOnlyDictionary<long, Minions> minionsList = GetMinions(log); //TODO(Rennorb @perf: find average complexity
            foreach (Minions mins in minionsList.Values)
            {
                DamageEvents.AddRange(mins.GetDamageEvents(null, log));
            }
            DamageEvents.SortByTime();
            DamageEventByDst = DamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        }
    }

    public IEnumerable<HealthDamageEvent> GetJustActorDamageEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        return GetDamageEvents(target, log, start, end).Where(x => x.From.Is(AgentItem));
    }
    public IEnumerable<HealthDamageEvent> GetJustActorDamageEvents(SingleActor? target, ParsedEvtcLog log)
    {
        return GetJustActorDamageEvents(target, log, log.LogData.LogStart, log.LogData.LogEnd);
    }

    protected override void InitDamageTakenEvents(ParsedEvtcLog log)
    {
        if (DamageTakenEvents == null || DamageTakenEventsBySrc == null)
        {
            DamageTakenEvents = [.. log.CombatData.GetDamageTakenData(AgentItem)];
            DamageTakenEventsBySrc = DamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        }
    }

    private readonly Dictionary<DamageType, CachingCollectionWithTarget<List<HealthDamageEvent>>> _typedSelfHitDamageEvents = [];
    /// <summary>
    /// cached method for damage modifiers
    /// </summary>
    internal IReadOnlyList<HealthDamageEvent> GetJustActorHitDamageEvents(SingleActor? target, ParsedEvtcLog log, long start, long end, DamageType damageType)
    {
        if (!_typedSelfHitDamageEvents.TryGetValue(damageType, out var hitDamageEventsPerPhasePerTarget))
        {
            hitDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<HealthDamageEvent>>(log);
            _typedSelfHitDamageEvents[damageType] = hitDamageEventsPerPhasePerTarget;
        }
        if (!hitDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<HealthDamageEvent>? dls))
        {
            dls = GetHitDamageEvents(target, log, start, end, damageType).Where(x => x.From.Is(AgentItem)).ToList();
            hitDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
        }
        return dls;
    }

    private readonly Dictionary<DamageType, CachingCollectionWithTarget<List<HealthDamageEvent>>> _typedMinionsHitDamageEvents = [];
    internal IReadOnlyList<HealthDamageEvent> GetJustMinionsHitDamageEvents(SingleActor? target, ParsedEvtcLog log, long start, long end, DamageType damageType)
    {
        if (!_typedMinionsHitDamageEvents.TryGetValue(damageType, out var hitDamageEventsPerPhasePerTarget))
        {
            hitDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<HealthDamageEvent>>(log);
            _typedMinionsHitDamageEvents[damageType] = hitDamageEventsPerPhasePerTarget;
        }
        if (!hitDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<HealthDamageEvent>? dls))
        {
            dls = GetHitDamageEvents(target, log, start, end, damageType).Where(x => !x.From.Is(AgentItem)).ToList();
            hitDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
        }
        return dls;
    }

    #endregion DAMAGE

    #region BREAKBAR DAMAGE

    public IEnumerable<BreakbarDamageEvent> GetJustActorBreakbarDamageEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        return GetBreakbarDamageEvents(target, log, start, end).Where(x => x.From.Is(AgentItem));
    }

#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitBreakbarDamageEvents(ParsedEvtcLog log)
    {
        if (BreakbarDamageEvents == null)
        {
            BreakbarDamageEvents = new List<BreakbarDamageEvent>(log.CombatData.GetBreakbarDamageData(AgentItem).Where(x => !x.ToFriendly));
            IReadOnlyDictionary<long, Minions> minionsList = GetMinions(log); //TODO(Rennorb) @perf: find average complexity
            foreach (Minions mins in minionsList.Values)
            {
                BreakbarDamageEvents.AddRange(mins.GetBreakbarDamageEvents(null, log));
            }
            BreakbarDamageEvents.SortByTime();
            BreakbarDamageEventsByDst = BreakbarDamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting

    public override IEnumerable<BreakbarDamageEvent> GetBreakbarDamageEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitBreakbarDamageEvents(log);

        if (target != null)
        {
            if (BreakbarDamageEventsByDst.TryGetValue(target.EnglobingAgentItem, out var list))
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

        return BreakbarDamageEvents.Where(x => x.Time >= start && x.Time <= end);
    }


#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitBreakbarDamageTakenEvents(ParsedEvtcLog log)
    {
        if (BreakbarDamageTakenEvents == null)
        {
            BreakbarDamageTakenEvents = new List<BreakbarDamageEvent>(log.CombatData.GetBreakbarDamageTakenData(AgentItem));
            BreakbarDamageTakenEventsBySrc = BreakbarDamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting
    public override IEnumerable<BreakbarDamageEvent> GetBreakbarDamageTakenEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitBreakbarDamageTakenEvents(log);

        if (target != null)
        {
            if (BreakbarDamageTakenEventsBySrc.TryGetValue(target.EnglobingAgentItem, out var list))
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

    public IEnumerable<CrowdControlEvent> GetJustOutgoingActorCrowdControlEvents(SingleActor target, ParsedEvtcLog log, long start, long end)
    {
        return GetOutgoingCrowdControlEvents(target, log, start, end).Where(x => x.From.Is(AgentItem));
    }

#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitOutgoingCrowdControlEvents(ParsedEvtcLog log)
    {
        if (OutgoingCrowdControlEvents == null)
        {
            OutgoingCrowdControlEvents = new List<CrowdControlEvent>(log.CombatData.GetOutgoingCrowdControlData(AgentItem).Where(x => !x.ToFriendly));
            IReadOnlyDictionary<long, Minions> minionsList = GetMinions(log);
            foreach (Minions mins in minionsList.Values)
            {
                OutgoingCrowdControlEvents.AddRange(mins.GetOutgoingCrowdControlEvents(null, log));
            }
            OutgoingCrowdControlEvents.SortByTime();
            OutgoingCrowdControlEventsByDst = OutgoingCrowdControlEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting}
    public override IEnumerable<CrowdControlEvent> GetOutgoingCrowdControlEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitOutgoingCrowdControlEvents(log);

        if (target != null)
        {
            if (OutgoingCrowdControlEventsByDst.TryGetValue(target.EnglobingAgentItem, out var ccList))
            {
                long targetStart = target.FirstAware;
                long targetEnd = target.LastAware;
                return ccList.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd);
            }
            else
            {
                return [ ];
            }
        }

        return OutgoingCrowdControlEvents.Where(x => x.Time >= start && x.Time <= end);
    }

#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitIncomingCrowdControlEvents(ParsedEvtcLog log)
    {
        if (IncomingCrowdControlEvents == null)
        {
            IncomingCrowdControlEvents = new List<CrowdControlEvent>(log.CombatData.GetIncomingCrowdControlData(AgentItem));
            IncomingCrowdControlEventsBySrc = IncomingCrowdControlEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting
    public override IEnumerable<CrowdControlEvent> GetIncomingCrowdControlEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitIncomingCrowdControlEvents(log);

        if (target != null)
        {
            if (IncomingCrowdControlEventsBySrc.TryGetValue(target.EnglobingAgentItem, out var list))
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
