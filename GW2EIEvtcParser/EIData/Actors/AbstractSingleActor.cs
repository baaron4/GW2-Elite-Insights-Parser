using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    public abstract class AbstractSingleActor : AbstractActor
    {
        public new AgentItem AgentItem => base.AgentItem;
        public string Account { get; protected set; }
        public int Group { get; protected set; }

        // Helpers
        private readonly SingleActorBuffsHelper _buffHelper;
        private readonly SingleActorGraphsHelper _graphHelper;
        private readonly SingleActorDamageModifierHelper _damageModifiersHelper;
        private readonly SingleActorStatusHelper _statusHelper;
        public EXTAbstractSingleActorHealingHelper EXTHealing { get; }
        public EXTAbstractSingleActorBarrierHelper EXTBarrier { get; }
        // Minions
        private Dictionary<long, Minions> _minions;
        // Replay
        private readonly Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>> _typedSelfHitDamageEvents = new Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>>();
        protected CombatReplay CombatReplay { get; set; }
        // Statistics
        private CachingCollectionWithTarget<FinalDPS> _dpsStats;
        private CachingCollectionWithTarget<FinalDefenses> _defenseStats;
        private CachingCollectionWithTarget<FinalOffensiveStats> _offensiveStats;
        private CachingCollection<FinalGameplayStats> _gameplayStats;
        private CachingCollectionWithTarget<FinalSupport> _supportStats;
        private CachingCollection<FinalToPlayersSupport> _toPlayerSupportStats;

        protected AbstractSingleActor(AgentItem agent) : base(agent)
        {
            Group = 51;
            Account = Character;
            _buffHelper = new SingleActorBuffsHelper(this);
            _graphHelper = new SingleActorGraphsHelper(this);
            _damageModifiersHelper = new SingleActorDamageModifierHelper(this);
            _statusHelper = new SingleActorStatusHelper(this);
            EXTHealing = new EXTAbstractSingleActorHealingHelper(this);
            EXTBarrier = new EXTAbstractSingleActorBarrierHelper(this);
        }

        // Status

        protected int Health { get; set; } = -2;

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

        internal abstract void SetManualHealth(int health, IReadOnlyList<(long hpValue, double percent)> hpDistribution = null);

        public virtual IReadOnlyList<(long hpValue, double percent)> GetHealthDistribution()
        {
            return null;
        }

        internal abstract void OverrideName(string name);

        public (IReadOnlyList<Segment> deads, IReadOnlyList<Segment> downs, IReadOnlyList<Segment> dcs) GetStatus(ParsedEvtcLog log)
        {
            return _statusHelper.GetStatus(log);
        }

        public (IReadOnlyList<Segment> breakbarNones, IReadOnlyList<Segment> breakbarActives, IReadOnlyList<Segment> breakbarImmunes, IReadOnlyList<Segment> breakbarRecoverings) GetBreakbarStatus(ParsedEvtcLog log)
        {
            return _statusHelper.GetBreakbarStatus(log);
        }

        public long GetTimeSpentInCombat(ParsedEvtcLog log, long start, long end)
        {
            return _statusHelper.GetTimeSpentInCombat(log, start, end);
        }

        public long GetActiveDuration(ParsedEvtcLog log, long start, long end)
        {
            return _statusHelper.GetActiveDuration(log, start, end);
        }
        public bool IsDownBefore90(ParsedEvtcLog log, long curTime)
        {
            return _statusHelper.IsDownBeforeNext90(log, curTime);
        }
        public bool IsDowned(ParsedEvtcLog log, long time)
        {
            (_, IReadOnlyList<Segment> downs, _) = _statusHelper.GetStatus(log);
            return downs.Any(x => x.ContainsPoint(time));
        }
        public bool IsDowned(ParsedEvtcLog log, long start, long end)
        {
            (_, IReadOnlyList<Segment> downs, _) = _statusHelper.GetStatus(log);
            return downs.Any(x => x.IntersectSegment(start, end));
        }
        public bool IsDead(ParsedEvtcLog log, long time)
        {
            (IReadOnlyList<Segment> deads, _, _) = _statusHelper.GetStatus(log);
            return deads.Any(x => x.ContainsPoint(time));
        }
        public bool IsDead(ParsedEvtcLog log, long start, long end)
        {
            (IReadOnlyList<Segment> deads, _, _) = _statusHelper.GetStatus(log);
            return deads.Any(x => x.IntersectSegment(start, end));
        }
        public bool IsDC(ParsedEvtcLog log, long time)
        {
            (_, _, IReadOnlyList<Segment> dcs) = _statusHelper.GetStatus(log);
            return dcs.Any(x => x.ContainsPoint(time));
        }
        public bool IsDC(ParsedEvtcLog log, long start, long end)
        {
            (_, _, IReadOnlyList<Segment> dcs) = _statusHelper.GetStatus(log);
            return dcs.Any(x => x.IntersectSegment(start, end));
        }

        public ArcDPSEnums.BreakbarState GetCurrentBreakbarState(ParsedEvtcLog log, long time)
        {
            (IReadOnlyList<Segment> nones, IReadOnlyList<Segment> actives, IReadOnlyList<Segment> immunes, IReadOnlyList<Segment> recoverings) = _statusHelper.GetBreakbarStatus(log);
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

        //


        public WeaponSets GetWeaponSets(ParsedEvtcLog log)
        {
            return _statusHelper.GetWeaponSets(log);
        }

        //
        public IReadOnlyList<Consumable> GetConsumablesList(ParsedEvtcLog log, long start, long end)
        {
            return _buffHelper.GetConsumablesList(log, start, end);
        }
        //
        public IReadOnlyList<DeathRecap> GetDeathRecaps(ParsedEvtcLog log)
        {
            return _statusHelper.GetDeathRecaps(log);
        }

        //

        public abstract string GetIcon();

        public IReadOnlyList<Segment> GetHealthUpdates(ParsedEvtcLog log)
        {
            return _graphHelper.GetHealthUpdates(log);
        }

        public double GetCurrentHealthPercent(ParsedEvtcLog log, long time)
        {
            return _graphHelper.GetCurrentHealthPercent(log, time);
        }

        /// <summary>
        /// Return the health value at requested %
        /// </summary>
        /// <param name="log"></param>
        /// <param name="currentHealthPercent"></param>
        /// <returns></returns>
        public abstract int GetCurrentHealth(ParsedEvtcLog log, double currentHealthPercent);

        /// <summary>
        /// Return the health value at requested time
        /// </summary>
        /// <param name="log"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetCurrentHealth(ParsedEvtcLog log, long time)
        {
            var currentHPPercent = GetCurrentHealthPercent(log, time);
            return GetCurrentHealth(log, currentHPPercent);
        }

        public IReadOnlyList<Segment> GetBreakbarPercentUpdates(ParsedEvtcLog log)
        {
            return _graphHelper.GetBreakbarPercentUpdates(log);
        }

        public IReadOnlyList<Segment> GetBarrierUpdates(ParsedEvtcLog log)
        {
            return _graphHelper.GetBarrierUpdates(log);
        }

        public double GetCurrentBarrierPercent(ParsedEvtcLog log, long time)
        {
            return _graphHelper.GetCurrentBarrierPercent(log, time);
        }

        /// <summary>
        /// Return the barrier value at requested %
        /// </summary>
        /// <param name="log"></param>
        /// <param name="currentBarrierPercent"></param>
        /// <param name="time">Time at which to check for barrier. Barrier scales of the current maximum health of the actor and maximum health can change dynamically</param>
        /// <returns></returns>
        public abstract int GetCurrentBarrier(ParsedEvtcLog log, double currentBarrierPercent, long time);

        /// <summary>
        /// Return the barrier value at requested time
        /// </summary>
        /// <param name="log"></param>
        /// <param name="time">Time at which to check for barrier. Barrier scales of the current maximum health of the actor and maximum health can change dynamically</param>
        /// <returns></returns>
        public int GetCurrentBarrier(ParsedEvtcLog log, long time)
        {
            var currentBarrierPercent = GetCurrentBarrierPercent(log, time);
            return GetCurrentBarrier(log, currentBarrierPercent, time);
        }

        // Minions
        public IReadOnlyDictionary<long, Minions> GetMinions(ParsedEvtcLog log)
        {
            if (_minions == null)
            {
                _minions = new Dictionary<long, Minions>();
                // npcs, species id based
                var combatMinion = log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.Master != null && x.GetFinalMaster() == AgentItem).ToList();
                var auxMinions = new Dictionary<long, Minions>();
                foreach (AgentItem agent in combatMinion)
                {
                    long id = agent.ID;
                    AbstractSingleActor singleActor = log.FindActor(agent);
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
                    AbstractSingleActor singleActor = log.FindActor(agent);
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

        // Graph
        public IReadOnlyList<int> Get1SDamageList(ParsedEvtcLog log, long start, long end, AbstractSingleActor target, ParserHelper.DamageType damageType)
        {
            return _graphHelper.Get1SDamageList(log, start, end, target, damageType);
        }

        public IReadOnlyList<int> Get1SDamageTakenList(ParsedEvtcLog log, long start, long end, AbstractSingleActor target, ParserHelper.DamageType damageType)
        {
            return _graphHelper.Get1SDamageTakenList(log, start, end, target, damageType);
        }

        public IReadOnlyList<double> Get1SBreakbarDamageList(ParsedEvtcLog log, long start, long end, AbstractSingleActor target)
        {
            return _graphHelper.Get1SBreakbarDamageList(log, start, end, target);
        }
        public IReadOnlyList<double> Get1SBreakbarDamageTakenList(ParsedEvtcLog log, long start, long end, AbstractSingleActor target)
        {
            return _graphHelper.Get1SBreakbarDamageTakenList(log, start, end, target);
        }

        // Damage Modifiers

        public IReadOnlyDictionary<string, DamageModifierStat> GetOutgoingDamageModifierStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            return _damageModifiersHelper.GetOutgoingDamageModifierStats(target, log, start, end);
        }

        public IReadOnlyCollection<string> GetPresentOutgoingDamageModifier(ParsedEvtcLog log)
        {
            return _damageModifiersHelper.GetPresentOutgoingDamageModifier(log);
        }

        public IReadOnlyDictionary<string, DamageModifierStat> GetIncomingDamageModifierStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            return _damageModifiersHelper.GetIncomingDamageModifierStats(target, log, start, end);
        }

        public IReadOnlyCollection<string> GetPresentIncomingDamageModifier(ParsedEvtcLog log)
        {
            return _damageModifiersHelper.GetPresentIncomingDamageModifier(log);
        }

        // Buffs
        public BuffDistribution GetBuffDistribution(ParsedEvtcLog log, long start, long end)
        {
            return _buffHelper.GetBuffDistribution(log, start, end);
        }

        public IReadOnlyDictionary<long, long> GetBuffPresence(ParsedEvtcLog log, long start, long end)
        {
            return _buffHelper.GetBuffPresence(log, start, end);
        }

        internal virtual Dictionary<long, FinalActorBuffs>[] ComputeBuffs(ParsedEvtcLog log, long start, long end, BuffEnum type)
        {
            Dictionary<long, FinalActorBuffs>[] empty =
            {
                        new Dictionary<long, FinalActorBuffs>(),
                        new Dictionary<long, FinalActorBuffs>()
             };
            switch (type)
            {
                case BuffEnum.Group:
                    return empty;
                case BuffEnum.OffGroup:
                    return empty;
                case BuffEnum.Squad:
                    var otherPlayers = log.PlayerList.Where(p => p != this).ToList();
                    return FinalActorBuffs.GetBuffsForPlayers(otherPlayers, log, AgentItem, start, end);
                case BuffEnum.Self:
                default:
                    return FinalActorBuffs.GetBuffsForSelf(log, this, start, end);
            }
        }

        internal virtual Dictionary<long, FinalActorBuffVolumes>[] ComputeBuffVolumes(ParsedEvtcLog log, long start, long end, BuffEnum type)
        {
            Dictionary<long, FinalActorBuffVolumes>[] empty =
            {
                        new Dictionary<long, FinalActorBuffVolumes>(),
                        new Dictionary<long, FinalActorBuffVolumes>()
             };
            switch (type)
            {
                case BuffEnum.Group:
                    return empty;
                case BuffEnum.OffGroup:
                    return empty;
                case BuffEnum.Squad:
                    var otherPlayers = log.PlayerList.Where(p => p != this).ToList();
                    return FinalActorBuffVolumes.GetBuffVolumesForPlayers(otherPlayers, log, AgentItem, start, end);
                case BuffEnum.Self:
                default:
                    return FinalActorBuffVolumes.GetBuffVolumesForSelf(log, this, start, end);
            }
        }


        public IReadOnlyDictionary<long, BuffsGraphModel> GetBuffGraphs(ParsedEvtcLog log)
        {
            return _buffHelper.GetBuffGraphs(log);
        }

        public IReadOnlyDictionary<long, BuffsGraphModel> GetBuffGraphs(ParsedEvtcLog log, AbstractSingleActor by)
        {
            return _buffHelper.GetBuffGraphs(log, by);
        }

        /// <summary>
        /// Checks if a buff is present on the actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
        /// </summary>
        /// <param name="log"></param>
        /// <param name="buffId"></param>
        /// <param name="time"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        public bool HasBuff(ParsedEvtcLog log, long buffId, long time, long window = 0)
        {
            return _buffHelper.HasBuff(log, buffId, time, window);
        }

        /// <summary>
        /// Checks if a buff is present on the actor and was applied by given actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
        /// </summary>
        /// <param name="log"></param>
        /// <param name="by"></param>
        /// <param name="buffId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool HasBuff(ParsedEvtcLog log, AbstractSingleActor by, long buffId, long time)
        {
            return _buffHelper.HasBuff(log, by, buffId, time);
        }

        public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, long buffId, long start, long end)
        {
            return _buffHelper.GetBuffStatus(log, buffId, start, end);
        }

        public Segment GetBuffStatus(ParsedEvtcLog log, long buffId, long time)
        {
            return _buffHelper.GetBuffStatus(log, buffId, time);
        }

        /// <summary>
        /// Creates a <see cref="List{T}"/> of <see cref="Segment"/> of the <paramref name="buffIds"/> in input.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="buffIds">Buff IDs of which to find the <see cref="Segment"/> of.</param>
        /// <param name="start">Start time to search.</param>
        /// <param name="end">End time to search.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> with the <see cref="Segment"/>s found.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, long[] buffIds, long start, long end)
        {
            var result = new List<Segment>();
            foreach (long id in buffIds)
            {
                result.AddRange(_buffHelper.GetBuffStatus(log, id, start, end));
            }
            return result;
        }

        public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, AbstractSingleActor by, long buffId, long start, long end)
        {
            return _buffHelper.GetBuffStatus(log, by, buffId, start, end);
        }

        public Segment GetBuffStatus(ParsedEvtcLog log, AbstractSingleActor by, long buffId, long time)
        {
            return _buffHelper.GetBuffStatus(log, by, buffId, time);
        }

        public IReadOnlyDictionary<long, FinalActorBuffs> GetBuffs(BuffEnum type, ParsedEvtcLog log, long start, long end)
        {
            return _buffHelper.GetBuffs(type, log, start, end);
        }

        public IReadOnlyDictionary<long, FinalActorBuffs> GetActiveBuffs(BuffEnum type, ParsedEvtcLog log, long start, long end)
        {
            return _buffHelper.GetActiveBuffs(type, log, start, end);
        }

        public IReadOnlyCollection<Buff> GetTrackedBuffs(ParsedEvtcLog log)
        {
            return _buffHelper.GetTrackedBuffs(log);
        }


        public IReadOnlyDictionary<long, FinalBuffsDictionary> GetBuffsDictionary(ParsedEvtcLog log, long start, long end)
        {
            return _buffHelper.GetBuffsDictionary(log, start, end);
        }

        public IReadOnlyDictionary<long, FinalBuffsDictionary> GetActiveBuffsDictionary(ParsedEvtcLog log, long start, long end)
        {
            return _buffHelper.GetActiveBuffsDictionary(log, start, end);
        }

        public IReadOnlyDictionary<long, FinalActorBuffVolumes> GetBuffVolumes(BuffEnum type, ParsedEvtcLog log, long start, long end)
        {
            return _buffHelper.GetBuffVolumes(type, log, start, end);
        }

        public IReadOnlyDictionary<long, FinalActorBuffVolumes> GetActiveBuffVolumes(BuffEnum type, ParsedEvtcLog log, long start, long end)
        {
            return _buffHelper.GetActiveBuffVolumes(type, log, start, end);
        }

        public IReadOnlyDictionary<long, FinalBuffVolumesDictionary> GetBuffVolumesDictionary(ParsedEvtcLog log, long start, long end)
        {
            return _buffHelper.GetBuffVolumesDictionary(log, start, end);
        }

        public IReadOnlyDictionary<long, FinalBuffVolumesDictionary> GetActiveBuffVolumesDictionary(ParsedEvtcLog log, long start, long end)
        {
            return _buffHelper.GetActiveBuffVolumesDictionary(log, start, end);
        }

        //
        protected void SetMovements(ParsedEvtcLog log)
        {
            foreach (AbstractMovementEvent movementEvent in log.CombatData.GetMovementData(AgentItem))
            {
                movementEvent.AddPoint3D(CombatReplay);
            }
        }

        public bool HasCombatReplayPositions(ParsedEvtcLog log)
        {
            return GetCombatReplayNonPolledPositions(log).Count > 0 && GetCombatReplayPolledPositions(log).Count > 0;
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
            foreach (string squadMarkerGUID in MarkerGUIDs.SquadOverheadMarkersHexGUIDs)
            {
                if (log.CombatData.TryGetMarkerEventsBySrcWithGUID(AgentItem, squadMarkerGUID, out IReadOnlyList<MarkerEvent> markerEvents))
                {
                    foreach (MarkerEvent markerEvent in markerEvents)
                    {
                        CombatReplay.AddRotatedOverheadMarkerIcon(new Segment(markerEvent.Time, markerEvent.EndTime, 1), this, ParserIcons.SquadMarkerGUIDsToIcon[squadMarkerGUID], 240f, 16, 1);
                    }
                }
            }
        }

        public abstract AbstractSingleActorCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log);

        // Cast logs
        public override IReadOnlyList<AbstractCastEvent> GetCastEvents(ParsedEvtcLog log, long start, long end)
        {
            if (CastEvents == null)
            {
                SetCastEvents(log);
            }
            return CastEvents.Where(x => x.Time >= start && x.Time <= end).ToList();

        }
        public override IReadOnlyList<AbstractCastEvent> GetIntersectingCastEvents(ParsedEvtcLog log, long start, long end)
        {
            if (CastEvents == null)
            {
                SetCastEvents(log);
            }
            return CastEvents.Where(x => KeepIntersectingCastLog(x, start, end)).ToList();

        }
        protected void SetCastEvents(ParsedEvtcLog log)
        {
            CastEvents = new List<AbstractCastEvent>(log.CombatData.GetAnimatedCastData(AgentItem));
            CastEvents.AddRange(log.CombatData.GetInstantCastData(AgentItem));
            foreach (WeaponSwapEvent wepSwap in log.CombatData.GetWeaponSwapData(AgentItem))
            {
                if (CastEvents.Count > 0 && (wepSwap.Time - CastEvents.Last().Time) < ServerDelayConstant && CastEvents.Last().SkillId == WeaponSwap)
                {
                    CastEvents[CastEvents.Count - 1] = wepSwap;
                }
                else
                {
                    CastEvents.Add(wepSwap);
                }
            }
            CastEvents = CastEvents.OrderBy(x => x.Time).ThenBy(x => !x.Skill.IsSwap).ToList();
        }

        // DPS Stats

        public FinalDPS GetDPSStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_dpsStats == null)
            {
                _dpsStats = new CachingCollectionWithTarget<FinalDPS>(log);
            }
            if (!_dpsStats.TryGetValue(start, end, target, out FinalDPS value))
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

        public FinalDefenses GetDefenseStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_defenseStats == null)
            {
                _defenseStats = new CachingCollectionWithTarget<FinalDefenses>(log);
            }
            if (!_defenseStats.TryGetValue(start, end, target, out FinalDefenses value))
            {
                value = target != null ? new FinalDefenses(log, start, end, this, target) : new FinalDefensesAll(log, start, end, this);
                _defenseStats.Set(start, end, target, value);
            }
            return value;
        }

        public FinalDefensesAll GetDefenseStats(ParsedEvtcLog log, long start, long end)
        {
            return GetDefenseStats(null, log, start, end) as FinalDefensesAll;
        }

        // Gameplay Stats

        public FinalGameplayStats GetGameplayStats(ParsedEvtcLog log, long start, long end)
        {
            if (_gameplayStats == null)
            {
                _gameplayStats = new CachingCollection<FinalGameplayStats>(log);
            }
            if (!_gameplayStats.TryGetValue(start, end, out FinalGameplayStats value))
            {
                value = new FinalGameplayStats(log, start, end, this);
                _gameplayStats.Set(start, end, value);
            }
            return value;
        }

        public FinalOffensiveStats GetOffensiveStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_offensiveStats == null)
            {
                _offensiveStats = new CachingCollectionWithTarget<FinalOffensiveStats>(log);
            }
            if (!_offensiveStats.TryGetValue(start, end, target, out FinalOffensiveStats value))
            {
                value = new FinalOffensiveStats(log, start, end, this, target);
                _offensiveStats.Set(start, end, target, value);
            }
            return value;
        }

        // Support stats
        public FinalSupportAll GetSupportStats(ParsedEvtcLog log, long start, long end)
        {
            return GetSupportStats(null, log, start, end) as FinalSupportAll;
        }

        public FinalSupport GetSupportStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_supportStats == null)
            {
                _supportStats = new CachingCollectionWithTarget<FinalSupport>(log);
            }
            if (!_supportStats.TryGetValue(start, end, target, out FinalSupport value))
            {
                value = target != null ? new FinalSupport(log, start, end, this, target) : new FinalSupportAll(log, start, end, this);
                _supportStats.Set(start, end, target, value);
            }
            return value;
        }

        public FinalToPlayersSupport GetToPlayerSupportStats(ParsedEvtcLog log, long start, long end)
        {
            if (_toPlayerSupportStats == null)
            {
                _toPlayerSupportStats = new CachingCollection<FinalToPlayersSupport>(log);
            }
            if (!_toPlayerSupportStats.TryGetValue(start, end, out FinalToPlayersSupport value))
            {
                value = new FinalToPlayersSupport(log, this, start, end);
                _toPlayerSupportStats.Set(start, end, value);
            }
            return value;
        }


        // Damage logs
        public override IReadOnlyList<AbstractHealthDamageEvent> GetDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (DamageEvents == null)
            {
                DamageEvents = new List<AbstractHealthDamageEvent>();
                DamageEvents.AddRange(log.CombatData.GetDamageData(AgentItem).Where(x => !x.ToFriendly));
                IReadOnlyDictionary<long, Minions> minionsList = GetMinions(log);
                foreach (Minions mins in minionsList.Values)
                {
                    DamageEvents.AddRange(mins.GetDamageEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd));
                }
                DamageEvents = DamageEvents.OrderBy(x => x.Time).ToList();
                DamageEventByDst = DamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageEventByDst.TryGetValue(target.AgentItem, out List<AbstractHealthDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<AbstractHealthDamageEvent>();
                }
            }
            return DamageEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public IReadOnlyList<AbstractHealthDamageEvent> GetJustActorDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            return GetDamageEvents(target, log, start, end).Where(x => x.From == AgentItem).ToList();
        }

        public IReadOnlyList<AbstractBreakbarDamageEvent> GetJustActorBreakbarDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            return GetBreakbarDamageEvents(target, log, start, end).Where(x => x.From == AgentItem).ToList();
        }

        public override IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (BreakbarDamageEvents == null)
            {
                BreakbarDamageEvents = new List<AbstractBreakbarDamageEvent>();
                BreakbarDamageEvents.AddRange(log.CombatData.GetBreakbarDamageData(AgentItem).Where(x => !x.ToFriendly));
                IReadOnlyDictionary<long, Minions> minionsList = GetMinions(log);
                foreach (Minions mins in minionsList.Values)
                {
                    BreakbarDamageEvents.AddRange(mins.GetBreakbarDamageEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd));
                }
                BreakbarDamageEvents = BreakbarDamageEvents.OrderBy(x => x.Time).ToList();
                BreakbarDamageEventsByDst = BreakbarDamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (BreakbarDamageEventsByDst.TryGetValue(target.AgentItem, out List<AbstractBreakbarDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<AbstractBreakbarDamageEvent>();
                }
            }
            return BreakbarDamageEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override IReadOnlyList<AbstractHealthDamageEvent> GetDamageTakenEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (DamageTakenEvents == null)
            {
                DamageTakenEvents = new List<AbstractHealthDamageEvent>();
                DamageTakenEvents.AddRange(log.CombatData.GetDamageTakenData(AgentItem));
                DamageTakenEvents = DamageTakenEvents.OrderBy(x => x.Time).ToList();
                DamageTakenEventsBySrc = DamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageTakenEventsBySrc.TryGetValue(target.AgentItem, out List<AbstractHealthDamageEvent> list))
                {
                    long targetStart = target.FirstAware;
                    long targetEnd = target.LastAware;
                    return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    return new List<AbstractHealthDamageEvent>();
                }
            }
            return DamageTakenEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageTakenEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (BreakbarDamageTakenEvents == null)
            {
                BreakbarDamageTakenEvents = new List<AbstractBreakbarDamageEvent>();
                BreakbarDamageTakenEvents.AddRange(log.CombatData.GetBreakbarDamageTakenData(AgentItem));
                BreakbarDamageTakenEvents = BreakbarDamageTakenEvents.OrderBy(x => x.Time).ToList();
                BreakbarDamageTakenEventsBySrc = BreakbarDamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (BreakbarDamageTakenEventsBySrc.TryGetValue(target.AgentItem, out List<AbstractBreakbarDamageEvent> list))
                {
                    long targetStart = target.FirstAware;
                    long targetEnd = target.LastAware;
                    return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    return new List<AbstractBreakbarDamageEvent>();
                }
            }
            return BreakbarDamageTakenEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        /// <summary>
        /// cached method for damage modifiers
        /// </summary>
        internal IReadOnlyList<AbstractHealthDamageEvent> GetJustActorHitDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end, ParserHelper.DamageType damageType)
        {
            if (!_typedSelfHitDamageEvents.TryGetValue(damageType, out CachingCollectionWithTarget<List<AbstractHealthDamageEvent>> hitDamageEventsPerPhasePerTarget))
            {
                hitDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>(log);
                _typedSelfHitDamageEvents[damageType] = hitDamageEventsPerPhasePerTarget;
            }
            if (!hitDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetHitDamageEvents(target, log, start, end, damageType).Where(x => x.From == AgentItem).ToList();
                hitDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }


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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="time"></param>
        /// <param name="forwardWindow">Position will be looked up to time + forwardWindow if given</param>
        /// <returns></returns>
        public Point3D GetCurrentPosition(ParsedEvtcLog log, long time, long forwardWindow = 0)
        {
            if (!HasCombatReplayPositions(log))
            {
                return null;
            }
            IReadOnlyList<ParametricPoint3D> positions = GetCombatReplayPolledPositions(log);
            if (forwardWindow != 0)
            {
                return positions.FirstOrDefault(x => x.Time >= time && x.Time <= time + forwardWindow) ?? positions.LastOrDefault(x => x.Time <= time);
            }
            int foundIndex = BinarySearchRecursive(positions, time, 0, positions.Count - 1);
            if (foundIndex < 0)
            {
                return null;
            }
            return positions[foundIndex];
        }

        public Point3D GetCurrentInterpolatedPosition(ParsedEvtcLog log, long time)
        {
            if (!HasCombatReplayPositions(log))
            {
                return null;
            }
            IReadOnlyList<ParametricPoint3D> positions = GetCombatReplayPolledPositions(log);
            ParametricPoint3D next = positions.FirstOrDefault(x => x.Time >= time);
            ParametricPoint3D prev = positions.LastOrDefault(x => x.Time <= time);
            Point3D res;
            if (prev != null && next != null)
            {
                long denom = next.Time - prev.Time;
                if (denom == 0)
                {
                    res = prev;
                }
                else
                {
                    float ratio = (float)(time - prev.Time) / denom;
                    res = new Point3D(prev, next, ratio);
                }
            }
            else
            {
                res = prev ?? next;
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="time"></param>
        /// <param name="forwardWindow">Rotation will be looked up to time + forwardWindow if given</param>
        /// <returns></returns>
        public Point3D GetCurrentRotation(ParsedEvtcLog log, long time, long forwardWindow = 0)
        {
            IReadOnlyList<ParametricPoint3D> rotations = GetCombatReplayPolledRotations(log);
            if (!rotations.Any())
            {
                return null;
            }
            if (forwardWindow != 0)
            {
                return rotations.FirstOrDefault(x => x.Time >= time && x.Time <= time + forwardWindow) ?? rotations.LastOrDefault(x => x.Time <= time);
            }
            int foundIndex = BinarySearchRecursive(rotations, time, 0, rotations.Count - 1);
            if (foundIndex < 0)
            {
                return null;
            }
            return rotations[foundIndex];
        }
    }
}
