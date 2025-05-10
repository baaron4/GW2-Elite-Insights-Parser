using GW2EIEvtcParser.Exceptions;
using static GW2EIEvtcParser.ArcDPSEnums;
using System.Diagnostics.CodeAnalysis;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData;

partial class CombatData
{
    public IReadOnlyCollection<long> GetSkills()
    {
        return _skillIds;
    }
    #region BUILD
    public EvtcVersionEvent GetEvtcVersionEvent()
    {
        return _metaDataEvents.EvtcVersionEvent!;
    }
    public GW2BuildEvent GetGW2BuildEvent()
    {
        if (_metaDataEvents.GW2BuildEvent == null)
        {
            throw new EvtcCombatEventException("Missing Build Event");
        }
        return _metaDataEvents.GW2BuildEvent;
    }
    #endregion BUILD
    #region STATUS
    public IReadOnlyList<AliveEvent> GetAliveEvents(AgentItem src)
    {
        return _statusEvents.AliveEvents.GetValueOrEmpty(src);
    }
    public IReadOnlyList<DeadEvent> GetDeadEvents(AgentItem src)
    {
        return _statusEvents.DeadEvents.GetValueOrEmpty(src);
    }
    public IReadOnlyList<DespawnEvent> GetDespawnEvents(AgentItem src)
    {
        return _statusEvents.DespawnEvents.GetValueOrEmpty(src);
    }
    public IReadOnlyList<DownEvent> GetDownEvents(AgentItem src)
    {
        return _statusEvents.DownEvents.GetValueOrEmpty(src);
    }
    public IReadOnlyList<SpawnEvent> GetSpawnEvents(AgentItem src)
    {
        return _statusEvents.SpawnEvents.GetValueOrEmpty(src);
    }
    #endregion STATUS
    #region ATTACKTARGETS
    public IReadOnlyList<AttackTargetEvent> GetAttackTargetEvents(AgentItem targetedAgent)
    {
        return _statusEvents.AttackTargetEvents.GetValueOrEmpty(targetedAgent);
    }

    public IReadOnlyList<AttackTargetEvent> GetAttackTargetEventsByAttackTarget(AgentItem attackTarget)
    {
        return _statusEvents.AttackTargetEventsByAttackTarget.GetValueOrEmpty(attackTarget);
    }
    public IReadOnlyList<TargetableEvent> GetTargetableEvents(AgentItem attackTarget)
    {
        return _statusEvents.TargetableEvents.GetValueOrEmpty(attackTarget);
    }
    #endregion ATTACKTARGETS
    #region DATE
    public InstanceStartEvent? GetInstanceStartEvent()
    {
        return _metaDataEvents.InstanceStartEvent;
    }

    public SquadCombatStartEvent? GetLogStartEvent()
    {
        return _metaDataEvents.LogStartEvent;
    }

    public IReadOnlyList<SquadCombatStartEvent> GetSquadCombatStartEvents()
    {
        return _metaDataEvents.SquadCombatStartEvents;
    }

    public IReadOnlyList<LogNPCUpdateEvent> GetLogNPCUpdateEvents()
    {
        return _metaDataEvents.LogNPCUpdateEvents;
    }

    public SquadCombatEndEvent? GetLogEndEvent()
    {
        return _metaDataEvents.LogEndEvent;
    }
    public IReadOnlyList<SquadCombatEndEvent> GetSquadCombatEndEvents()
    {
        return _metaDataEvents.SquadCombatEndEvents;
    }
    #endregion DATE

    public IReadOnlyList<GuildEvent> GetGuildEvents(AgentItem src)
    {
        return _metaDataEvents.GuildEvents.GetValueOrEmpty(src);
    }

    public PointOfViewEvent? GetPointOfViewEvent()
    {
        return _metaDataEvents.PointOfViewEvent;
    }
    public FractalScaleEvent? GetFractalScaleEvent()
    {
        return _metaDataEvents.FractalScaleEvent;
    }
    public LanguageEvent? GetLanguageEvent()
    {
        return _metaDataEvents.LanguageEvent;
    }
    public IReadOnlyList<MapIDEvent> GetMapIDEvents()
    {
        return _metaDataEvents.MapIDEvents;
    }

    public IReadOnlyList<RewardEvent> GetRewardEvents()
    {
        return _rewardEvents;
    }

    public IReadOnlyList<ErrorEvent> GetErrorEvents()
    {
        return _metaDataEvents.ErrorEvents;
    }

    public IReadOnlyList<ShardEvent> GetShardEvents()
    {
        return _metaDataEvents.ShardEvents;
    }

    public IReadOnlyList<TickRateEvent> GetTickRateEvents()
    {
        return _metaDataEvents.TickRateEvents;
    }

    #region MARKERS

    /// <summary>
    /// Returns squad marker events of given marker index
    /// </summary>
    /// <param name="markerIndex">marker index</param>
    /// <returns></returns>
    public IReadOnlyList<SquadMarkerEvent> GetSquadMarkerEvents(SquadMarkerIndex markerIndex)
    {
        return _statusEvents.SquadMarkerEventsByIndex.GetValueOrEmpty(markerIndex);
    }
    /// <summary>
    /// Returns marker events owned by agent
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    public IReadOnlyList<MarkerEvent> GetMarkerEvents(AgentItem agent)
    {
        return _statusEvents.MarkerEventsBySrc.GetValueOrEmpty(agent);
    }
    /// <summary>
    /// Returns marker events of given marker ID
    /// </summary>
    /// <param name="markerID">marker ID</param>
    /// <returns></returns>
    public IReadOnlyList<MarkerEvent> GetMarkerEventsByMarkerID(long markerID)
    {
        return _statusEvents.MarkerEventsByID.GetValueOrEmpty(markerID);
    }
    /// <summary>
    /// True if marker events of given marker GUID has been found
    /// </summary>
    /// <param name="marker">marker GUID</param>
    /// <param name="markerEvents">Found marker events</param>
    public bool TryGetMarkerEventsByGUID(GUID marker, [NotNullWhen(true)] out IReadOnlyList<MarkerEvent>? markerEvents)
    {
        var markerGUIDEvent = GetMarkerGUIDEvent(marker);
        markerEvents = GetMarkerEventsByMarkerID(markerGUIDEvent.ContentID);
        if (markerEvents.Count > 0)
        {
            return true;
        }
        markerEvents = null;
        return false;
    }
    /// <summary>
    /// True if marker events of given marker GUID has been found on given agent
    /// </summary>
    /// <param name="agent">marker owner</param>
    /// <param name="marker">marker GUID</param>
    /// <param name="markerEvents">Found marker events</param>
    public bool TryGetMarkerEventsBySrcWithGUID(AgentItem agent, GUID marker, [NotNullWhen(true)] out IReadOnlyList<MarkerEvent>? markerEvents)
    {
        if (TryGetMarkerEventsByGUID(marker, out var markers))
        {
            markerEvents = markers.Where(effect => effect.Src == agent).ToList();
            return true;
        }
        markerEvents = null;
        return false;
    }

    #endregion MARKERS
    
    #region STATES

    public IReadOnlyList<BarrierUpdateEvent> GetBarrierUpdateEvents(AgentItem src)
    {
        return _statusEvents.BarrierUpdateEvents.GetValueOrEmpty(src);
    }

    public IReadOnlyList<MaxHealthUpdateEvent> GetMaxHealthUpdateEvents(AgentItem src)
    {
        return _statusEvents.MaxHealthUpdateEvents.GetValueOrEmpty(src);
    }
    public IReadOnlyList<HealthUpdateEvent> GetHealthUpdateEvents(AgentItem src)
    {
        return _statusEvents.HealthUpdateEvents.GetValueOrEmpty(src);
    }
    public IReadOnlyList<BreakbarStateEvent> GetBreakbarStateEvents(AgentItem src)
    {
        return _statusEvents.BreakbarStateEvents.GetValueOrEmpty(src);
    }

    public IReadOnlyList<BreakbarPercentEvent> GetBreakbarPercentEvents(AgentItem src)
    {
        return _statusEvents.BreakbarPercentEvents.GetValueOrEmpty(src);
    }
    public IReadOnlyList<EnterCombatEvent> GetEnterCombatEvents(AgentItem src)
    {
        return _statusEvents.EnterCombatEvents.GetValueOrEmpty(src);
    }
    public IReadOnlyList<ExitCombatEvent> GetExitCombatEvents(AgentItem src)
    {
        return _statusEvents.ExitCombatEvents.GetValueOrEmpty(src);
    }
    public IReadOnlyList<TeamChangeEvent> GetTeamChangeEvents(AgentItem src)
    {
        return _statusEvents.TeamChangeEvents.GetValueOrEmpty(src);
    }
    #endregion STATES

    #region INFO

    public BuffInfoEvent? GetBuffInfoEvent(long buffID)
    {
        return _metaDataEvents.BuffInfoEvents.GetValueOrDefault(buffID);
    }

    public IReadOnlyList<BuffInfoEvent> GetBuffInfoEvent(byte category)
    {
        return _metaDataEvents.BuffInfoEventsByCategory.GetValueOrEmpty(category);
    }

    public SkillInfoEvent? GetSkillInfoEvent(long skillID)
    {
        return _metaDataEvents.SkillInfoEvents.GetValueOrDefault(skillID);
    }
    #endregion INFO
    #region LAST90
    public IReadOnlyList<Last90BeforeDownEvent> GetLast90BeforeDownEvents()
    {
        return _statusEvents.Last90BeforeDownEvents;
    }

    public IReadOnlyList<Last90BeforeDownEvent> GetLast90BeforeDownEvents(AgentItem src)
    {
        return _statusEvents.Last90BeforeDownEventsBySrc.GetValueOrEmpty(src);
    }
    #endregion LAST90
    #region BUFFS
    public IReadOnlyList<BuffEvent> GetBuffData(long buffID)
    {
        return _buffData.GetValueOrEmpty(buffID);
    }
    public IReadOnlyList<AbstractBuffApplyEvent> GetBuffApplyData(long buffID)
    {
        return _buffApplyData.GetValueOrEmpty(buffID);
    }
    /// <summary>
    /// Won't return Buff Extension events
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    public IReadOnlyList<BuffEvent> GetBuffDataBySrc(AgentItem src)
    {
        return _buffDataBySrc.GetValueOrEmpty(src);
    }
    /// <summary>
    /// Returns list of buff events applied on agent for given id
    /// </summary>
    public IReadOnlyList<BuffEvent> GetBuffDataByIDByDst(long buffID, AgentItem dst)
    {
        if (_buffDataByIDByDst.TryGetValue(buffID, out var agentDict))
        {
            if (agentDict.TryGetValue(dst, out var res))
            {
                return res;
            }
        }
        return [];
    }
    /// <summary>
    /// Returns list of buff apply events applied on agent for given id
    /// </summary>
    public IReadOnlyList<AbstractBuffApplyEvent> GetBuffApplyDataByIDByDst(long buffID, AgentItem dst)
    {
        if (_buffApplyDataByIDByDst.TryGetValue(buffID, out var agentDict))
        {
            if (agentDict.TryGetValue(dst, out var res))
            {
                return res;
            }
        }
        return [];
    }
    public IReadOnlyList<BuffEvent> GetBuffDataByInstanceID(long buffID, uint instanceID)
    {
        if (instanceID == 0)
        {
            return GetBuffData(buffID);
        }
        if (_buffDataByInstanceID.TryGetValue(buffID, out var dict))
        {
            if (dict.TryGetValue(instanceID, out var buffEventList))
            {
                return buffEventList;
            }
        }
        return [];
    }
    public IReadOnlyList<BuffRemoveAllEvent> GetBuffRemoveAllData(long buffID)
    {
        return _buffRemoveAllData.GetValueOrEmpty(buffID);
    }
    public IReadOnlyList<BuffRemoveAllEvent> GetBuffRemoveAllData(long buffID, AgentItem src)
    {
        if (_buffRemoveAllDataBySrc.TryGetValue(buffID, out var bySrc))
        {
            return bySrc.GetValueOrEmpty(src);
        }
        return [];
    }

    /// <summary>
    /// Returns list of buff events applied on agent
    /// </summary>
    public IReadOnlyList<BuffEvent> GetBuffDataByDst(AgentItem dst)
    {
        return _buffDataByDst.GetValueOrEmpty(dst);
    }
    #endregion BUFFS
    #region DAMAGE
    /// <summary>
    /// Returns list of damage events done by agent
    /// </summary>
    public IReadOnlyList<HealthDamageEvent> GetDamageData(AgentItem src)
    {
        return _damageData.GetValueOrEmpty(src);
    }

    /// <summary>
    /// Returns list of breakbar damage events done by agent
    /// </summary>
    public IReadOnlyList<BreakbarDamageEvent> GetBreakbarDamageData(AgentItem src)
    {
        return _breakbarDamageData.GetValueOrEmpty(src);
    }
    /// <summary>
    /// Returns list of breakbar damage events done by skill id
    /// </summary>
    public IReadOnlyList<BreakbarDamageEvent> GetBreakbarDamageData(long skillID)
    {
        return _breakbarDamageDataById.GetValueOrEmpty(skillID);
    }
    /// <summary>
    /// Returns list of damage events applied by a skill
    /// </summary>
    public IReadOnlyList<HealthDamageEvent> GetDamageData(long skillID)
    {
        return _damageDataById.GetValueOrEmpty(skillID);
    }
    /// <summary>
    /// Returns list of damage events taken by Agent
    /// </summary>
    public IReadOnlyList<HealthDamageEvent> GetDamageTakenData(AgentItem dst)
    {
        return _damageTakenData.GetValueOrEmpty(dst);
    }

    /// <summary>
    /// Returns list of breakbar damage events taken by Agent
    /// </summary>
    public IReadOnlyList<BreakbarDamageEvent> GetBreakbarDamageTakenData(AgentItem dst)
    {
        return _breakbarDamageTakenData.GetValueOrEmpty(dst);
    }
    #endregion DAMAGE
    #region CROWDCONTROL
    /// <summary>
    /// Returns list of crowd control events done by agent
    /// </summary>
    public IReadOnlyList<CrowdControlEvent> GetOutgoingCrowdControlData(AgentItem src)
    {
        return _crowControlData.GetValueOrEmpty(src);
    }
    /// <summary>
    /// Returns list of crowd control events done by skill id
    /// </summary>
    public IReadOnlyList<CrowdControlEvent> GetCrowdControlData(long skillID)
    {
        return _crowControlDataById.GetValueOrEmpty(skillID);
    }

    /// <summary>
    /// Returns list of crowd control events taken by Agent
    /// </summary>
    public IReadOnlyList<CrowdControlEvent> GetIncomingCrowdControlData(AgentItem dst)
    {
        return _crowControlTakenData.GetValueOrEmpty(dst);
    }

    public IReadOnlyList<StunBreakEvent> GetStunBreakEvents(AgentItem src)
    {
        return _statusEvents.StunBreakEventsBySrc.GetValueOrEmpty(src);
    }
    #endregion CROWDCONTROL
    #region CAST
    /// <summary>
    /// Returns list of animated cast events done by Agent
    /// </summary>
    public IReadOnlyList<AnimatedCastEvent> GetAnimatedCastData(AgentItem caster)
    {
        return _animatedCastData.GetValueOrEmpty(caster);
    }

    /// <summary>
    /// Returns list of instant cast events done by Agent
    /// </summary>
    public IReadOnlyList<InstantCastEvent> GetInstantCastData(AgentItem caster)
    {
        return _instantCastData.GetValueOrEmpty(caster);
    }
    /// <summary>
    /// Returns list of instant cast events done by Agent
    /// </summary>
    public IReadOnlyList<InstantCastEvent> GetInstantCastData(long skillID)
    {
        return _instantCastDataById.GetValueOrEmpty(skillID);
    }

    /// <summary>
    /// Returns list of weapon swap events done by Agent
    /// </summary>
    public IReadOnlyList<WeaponSwapEvent> GetWeaponSwapData(AgentItem caster)
    {
        return _weaponSwapData.GetValueOrEmpty(caster);
    }
    /// <summary>
    /// Returns list of cast events from skill
    /// </summary>
    public IReadOnlyList<AnimatedCastEvent> GetAnimatedCastData(long skillID)
    {
        return _animatedCastDataById.GetValueOrEmpty(skillID);
    }
    #endregion CAST
    #region MOVEMENTS
    public IReadOnlyList<MovementEvent> GetMovementData(AgentItem src)
    {
        return _statusEvents.MovementEvents.GetValueOrEmpty(src);
    }
    public IReadOnlyList<GliderEvent> GetGliderEvents(AgentItem src)
    {
        return _statusEvents.GliderEventsBySrc.GetValueOrEmpty(src);
    }
    #endregion MOVEMENTS
    #region EFFECTS
    public IReadOnlyList<EffectEvent> GetEffectEventsBySrc(AgentItem src)
    {
        return _statusEvents.EffectEventsBySrc.GetValueOrEmpty(src);
    }

    public IReadOnlyList<EffectEvent> GetEffectEventsByDst(AgentItem dst)
    {
        return _statusEvents.EffectEventsByDst.GetValueOrEmpty(dst);
    }

    public IReadOnlyList<EffectEvent> GetEffectEventsByEffectID(long effectID)
    {
        return _statusEvents.EffectEventsByEffectID.GetValueOrEmpty(effectID);
    }

    /// <returns>true on success</returns>
    public bool TryGetEffectEventsByGUID(GUID effectGUID, [NotNullWhen(true)] out IReadOnlyList<EffectEvent>? effectEvents)
    {
        var effectGUIDEvent = GetEffectGUIDEvent(effectGUID);
        effectEvents = GetEffectEventsByEffectID(effectGUIDEvent.ContentID);
        if (effectEvents.Count > 0)
        {
            return true;
        }
        effectEvents = null;
        return false;
    }

    /// <returns>true on success</returns>
    public bool TryGetEffectEventsByGUIDs(Span<GUID> effects, out List<EffectEvent> effectEvents)
    {
        //TODO(Rennorb) @perf: fid average complexity
        effectEvents = new(effects.Length * 10);
        foreach (var effectGUID in effects)
        {
            if (TryGetEffectEventsByGUID(effectGUID, out var events))
            {
                effectEvents.AddRange(events);
            }
        }

        return effectEvents.Count > 0;
    }

    /// <summary>
    /// Returns effect events by the given agent and effect GUID.
    /// </summary>
    /// <returns>true on found effect with entries > 0</returns>
    public bool TryGetEffectEventsBySrcWithGUID(AgentItem agent, GUID effect, [NotNullWhen(true)] out IReadOnlyList<EffectEvent>? effectEvents)
    {
        if (TryGetEffectEventsByGUID(effect, out var effects))
        {
            var result = effects.Where(effect => effect.Src == agent).ToList();
            if (result.Count > 0)
            {
                effectEvents = result;
                return true;
            }
        }

        effectEvents = null;
        return false;
    }

    /// <summary>
    /// Appends effect events by the given agent and effect GUID.
    /// </summary>
    public void AppendEffectEventsBySrcWithGUID(AgentItem agent, GUID effect, List<EffectEvent> effectEvents)
    {
        if (TryGetEffectEventsByGUID(effect, out var effects))
        {
            effectEvents.AddRange(effects.Where(effect => effect.Src == agent));
        }
    }


    /// <summary>
    /// Returns effect events on the given agent and effect GUID.
    /// </summary>
    /// <returns>true on success</returns>
    public bool TryGetEffectEventsByDstWithGUID(AgentItem agent, GUID effect, [NotNullWhen(true)] out IReadOnlyList<EffectEvent>? effectEvents)
    {
        if (TryGetEffectEventsByGUID(effect, out var effects))
        {
            var result = effects.Where(effect => effect.Dst == agent).ToList();
            if (result.Count > 0)
            {
                effectEvents = result;
                return true;
            }
        }

        effectEvents = null;
        return false;
    }
    /// <summary>
    /// Append effect events on the given agent and effect GUID.
    /// </summary>
    public void AppendEffectEventsByDstWithGUID(AgentItem agent, GUID effect, List<EffectEvent> effectEvents)
    {
        if (TryGetEffectEventsByGUID(effect, out var effects))
        {
            effectEvents.AddRange(effects.Where(effect => effect.Dst == agent));
        }
    }

    /// <summary>
    /// Returns effect events by the given agent and effect GUIDs.
    /// </summary>
    /// <returns>true on success</returns>
    public bool TryGetEffectEventsBySrcWithGUIDs(AgentItem agent, ReadOnlySpan<GUID> effects, out List<EffectEvent> effectEvents)
    {
        //TODO(Rennorb) @perf: find average complexity
        effectEvents = new List<EffectEvent>(effects.Length * 10);
        foreach (var effectGUID in effects)
        {
            AppendEffectEventsBySrcWithGUID(agent, effectGUID, effectEvents);
        }

        return effectEvents.Count > 0;
    }
    /// <summary>
    /// Returns effect events on the given agent and effect GUIDs.
    /// </summary>
    /// <returns>true on success</returns>
    public bool TryGetEffectEventsByDstWithGUIDs(AgentItem agent, ReadOnlySpan<GUID> effects, out List<EffectEvent> effectEvents)
    {
        //TODO(Rennorb) @perf: find average complexity
        effectEvents = new List<EffectEvent>(effects.Length * 10);
        foreach (var effectGUID in effects)
        {
            AppendEffectEventsByDstWithGUID(agent, effectGUID, effectEvents);
        }

        return effectEvents.Count > 0;
    }

    /// <summary>
    /// Returns effect events by the given agent <b>including</b> minions and the given effect GUID.
    /// </summary>
    /// <returns>true on success</returns>
    public bool TryGetEffectEventsByMasterWithGUID(AgentItem agent, GUID effect, [NotNullWhen(true)] out IReadOnlyList<EffectEvent>? effectEvents)
    {
        if (TryGetEffectEventsByGUID(effect, out var effects))
        {
            var result = effects.Where(effect => effect.Src.GetFinalMaster() == agent).ToList();
            if (result.Count > 0)
            {
                effectEvents = result;
                return true;
            }
        }

        effectEvents = null;
        return false;
    }

    /// <summary>
    /// Returns effect events by the given agent <b>including</b> minions and the given effect GUID.
    /// </summary>
    public void AppendEffectEventsByMasterWithGUID(AgentItem agent, GUID effect, List<EffectEvent> effectEvents)
    {
        if (TryGetEffectEventsByGUID(effect, out var effects))
        {
            effectEvents.AddRange(effects.Where(effect => effect.Src.GetFinalMaster() == agent));
        }
    }

    /// <summary>
    /// Returns effect events by the given agent <b>including</b> minions and the given effect GUIDs.
    /// </summary>
    /// <returns>true on success</returns>
    public bool TryGetEffectEventsByMasterWithGUIDs(AgentItem agent, Span<GUID> effects, out List<EffectEvent> effectEvents)
    {
        effectEvents = [];
        foreach (var effectGUID in effects)
        {
            AppendEffectEventsByMasterWithGUID(agent, effectGUID, effectEvents);
        }

        return effectEvents.Count > 0;
    }

    /// <summary>
    /// Returns effect events by the given agent and effect GUID.
    /// Effects happening within epsilon milliseconds are grouped together.
    /// </summary>
    /// <param name="epsilon">Windows size</param>
    /// <returns>true on success</returns>
    public bool TryGetGroupedEffectEventsBySrcWithGUID(AgentItem agent, GUID effect, [NotNullWhen(true)] out List<List<EffectEvent>>? groupedEffectEvents, long epsilon = ServerDelayConstant)
    {
        if (!TryGetEffectEventsBySrcWithGUID(agent, effect, out var effects))
        {
            groupedEffectEvents = null;
            return false;
        }
        groupedEffectEvents = EpsilonWindowOverTime(effects, epsilon);

        return true;
    }
    /// <summary>
    /// Returns effect events for the given effect GUID.
    /// Effects happening within epsilon milliseconds are grouped together.
    /// </summary>
    /// <param name="epsilon">Window size</param>
    /// <returns>true on success</returns>
    public bool TryGetGroupedEffectEventsByGUID(GUID effect, [NotNullWhen(true)] out List<List<EffectEvent>>? groupedEffectEvents, long epsilon = ServerDelayConstant)
    {
        if (!TryGetEffectEventsByGUID(effect, out var effects))
        {
            groupedEffectEvents = null;
            return false;
        }

        groupedEffectEvents = EpsilonWindowOverTime(effects, epsilon);

        return true;
    }

    static List<List<EffectEvent>> EpsilonWindowOverTime(IReadOnlyList<EffectEvent> effectEvents, long epsilon)
    {
        //NOTE(Rennorb): Has entries due to invariant on TryGetEffectEventsBySrcWithGUID
        var startTime = effectEvents[0].Time;
        var endTime = effectEvents[^1].Time;
        var slices = Math.Max(1, (int)((endTime - startTime + (epsilon - 1)) / epsilon)); // ceiling of total duration / epsilon, and at least one slice
        var groupedEffectEvents = new List<List<EffectEvent>>(slices);


        var blockStart = startTime;
        var blockEnd = blockStart + epsilon;
        var currentBlock = new List<EffectEvent>(effectEvents.Count / slices); // assume average distribution
        int index = 0;
        foreach (var effectEvent in effectEvents)
        {
            if (effectEvent.Time >= blockEnd)
            {
                groupedEffectEvents.Add(currentBlock);
                currentBlock = new((effectEvents.Count - index) / slices); // assume average distribution in remaining blocks
                blockStart = effectEvent.Time;
                blockEnd = blockStart + epsilon;
            }

            currentBlock.Add(effectEvent);
            index++;
        }
        groupedEffectEvents.Add(currentBlock);

        return groupedEffectEvents;
    }


    public IReadOnlyList<EffectEvent> GetEffectEvents()
    {
        return _statusEvents.EffectEvents;
    }
    #endregion EFFECTS
    #region GUIDS
    public EffectGUIDEvent GetEffectGUIDEvent(GUID effectGUID)
    {
        return _metaDataEvents.EffectGUIDEventsByGUID.TryGetValue(effectGUID, out var evt) ? evt : EffectGUIDEvent.DummyEffectGUID;
    }

    internal EffectGUIDEvent GetEffectGUIDEvent(long effectID)
    {
        if (_metaDataEvents.EffectGUIDEventsByEffectID.TryGetValue(effectID, out var evt))
        {
            return evt;
        }
#if DEBUG2
        if (GetEffectEventsByEffectID(effectID).Count > 0)
        {
            throw new EvtcCombatEventException("Missing GUID event for effect " + effectID);
        }
#endif
        return EffectGUIDEvent.DummyEffectGUID;
    }

    public SkillGUIDEvent? GetSkillGUIDEvent(GUID skill)
    {
        return _metaDataEvents.SkillGUIDEventsByGUID.TryGetValue(skill, out var evt) ? evt : HasSpeciesAndSkillGUIDs ? SkillGUIDEvent.DummySkillGUID : null;
    }

    internal SkillGUIDEvent? GetSkillGUIDEvent(long skillID)
    {
        return _metaDataEvents.SkillGUIDEventsBySkillID.TryGetValue(skillID, out var evt) ? evt : HasSpeciesAndSkillGUIDs ? SkillGUIDEvent.DummySkillGUID : null;
    }

    public SpeciesGUIDEvent? GetSpeciesGUIDEvent(GUID species)
    {
        return _metaDataEvents.SpeciesGUIDEventsByGUID.TryGetValue(species, out var evt) ? evt : HasSpeciesAndSkillGUIDs ? SpeciesGUIDEvent.DummySpeciesGUID : null;
    }

    internal SpeciesGUIDEvent? GetSpeciesGUIDEvent(long speciesID)
    {
        return _metaDataEvents.SpeciesGUIDEventsBySpeciesID.TryGetValue(speciesID, out var evt) ? evt : HasSpeciesAndSkillGUIDs ? SpeciesGUIDEvent.DummySpeciesGUID : null;
    }

    public MarkerGUIDEvent GetMarkerGUIDEvent(GUID marker)
    {
        return _metaDataEvents.MarkerGUIDEventsByGUID.TryGetValue(marker, out var evt) ? evt : MarkerGUIDEvent.DummyMarkerGUID;
    }

    internal MarkerGUIDEvent GetMarkerGUIDEvent(long markerID)
    {
        return _metaDataEvents.MarkerGUIDEventsByMarkerID.TryGetValue(markerID, out var evt) ? evt : MarkerGUIDEvent.DummyMarkerGUID;
    }
    public MissileGUIDEvent GetMissileGUIDEvent(GUID missile)
    {
        return _metaDataEvents.MissileGUIDEventsByGUID.TryGetValue(missile, out var evt) ? evt : MissileGUIDEvent.DummyMissileGUID;
    }

    internal MissileGUIDEvent GetMissileGUIDEvent(long missileID)
    {
        return _metaDataEvents.MissileGUIDEventsByMissileID.TryGetValue(missileID, out var evt) ? evt : MissileGUIDEvent.DummyMissileGUID;
    }
    #endregion GUIDS
    #region MISSILE

    public IReadOnlyList<MissileEvent> GetMissileEvents()
    {
        return _statusEvents.MissileEvents;
    }
    public IReadOnlyList<MissileEvent> GetMissileEventsBySrc(AgentItem src)
    {
        return _statusEvents.MissileEventsBySrc.GetValueOrEmpty(src);
    }
    public IReadOnlyList<MissileEvent> GetMissileEventsBySkillID(long skillID)
    {
        return _statusEvents.MissileEventsBySkillID.GetValueOrEmpty(skillID);
    }

    public IReadOnlyList<MissileEvent> GetMissileEventsByMissileID(long missileID)
    {
        return _statusEvents.MissileEventsByMissileID.GetValueOrEmpty(missileID);
    }
    public IReadOnlyList<MissileLaunchEvent> GetMissileLaunchEventsByDst(AgentItem dst)
    {
        return _statusEvents.MissileLaunchEventsByDst.GetValueOrEmpty(dst);
    }
    public IReadOnlyList<MissileEvent> GetMissileDamagingEventsByDst(AgentItem dst)
    {
        return _statusEvents.MissileDamagingEventsByDst.GetValueOrEmpty(dst);
    }
    public IReadOnlyList<MissileEvent> GetMissileDamagingEventsBySrc(AgentItem src)
    {
        return _statusEvents.MissileDamagingEventsBySrc.GetValueOrEmpty(src);
    }
    public bool TryGetMissileEventsByGUID(GUID missileGUID, [NotNullWhen(true)] out IReadOnlyList<MissileEvent>? missileEvents)
    {
        var missileGUIDEvent = GetMissileGUIDEvent(missileGUID);
        missileEvents = GetMissileEventsByMissileID(missileGUIDEvent.ContentID);
        if (missileEvents.Count > 0)
        {
            return true;
        }
        missileEvents = null;
        return false;
    }
    #endregion MISSILE
}
