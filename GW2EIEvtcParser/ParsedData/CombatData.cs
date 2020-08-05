using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIControllers;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class CombatData
    {
        public bool HasMovementData { get; }

        //private List<CombatItem> _healingData;
        //private List<CombatItem> _healingReceivedData;
        private readonly StatusEventsContainer _statusEvents = new StatusEventsContainer();
        private readonly MetaEventsContainer _metaDataEvents = new MetaEventsContainer();
        private readonly HashSet<long> _skillIds;
        private readonly Dictionary<long, List<AbstractBuffEvent>> _buffData;
        private readonly Dictionary<long, List<BuffRemoveAllEvent>> _buffRemoveAllData;
        private readonly Dictionary<AgentItem, List<AbstractBuffEvent>> _buffDataByDst;
        private readonly Dictionary<AgentItem, List<AbstractDamageEvent>> _damageData;
        private readonly Dictionary<long, List<AbstractDamageEvent>> _damageDataById;
        private readonly Dictionary<AgentItem, List<AnimatedCastEvent>> _animatedCastData;
        private readonly Dictionary<AgentItem, List<WeaponSwapEvent>> _weaponSwapData;
        private readonly Dictionary<long, List<AbstractCastEvent>> _castDataById;
        private readonly Dictionary<AgentItem, List<AbstractDamageEvent>> _damageTakenData;
        private readonly Dictionary<AgentItem, List<AbstractMovementEvent>> _movementData;
        private readonly List<RewardEvent> _rewardEvents = new List<RewardEvent>();

        internal bool HasStackIDs { get; } = false;

        private void EIBuffParse(List<Player> players, SkillData skillData, FightData fightData)
        {
            var toAdd = new List<AbstractBuffEvent>();
            foreach (Player p in players)
            {
                if (p.Prof == "Weaver")
                {
                    toAdd = WeaverHelper.TransformWeaverAttunements(GetBuffData(p.AgentItem), p.AgentItem, skillData);
                }
                if (p.Prof == "Elementalist" || p.Prof == "Tempest")
                {
                    ElementalistHelper.RemoveDualBuffs(GetBuffData(p.AgentItem), skillData);
                }
            }
            toAdd.AddRange(fightData.Logic.SpecialBuffEventProcess(_buffDataByDst, _buffData, skillData));
            var buffIDsToSort = new HashSet<long>();
            var buffAgentsToSort = new HashSet<AgentItem>();
            foreach (AbstractBuffEvent bf in toAdd)
            {
                if (_buffDataByDst.TryGetValue(bf.To, out List<AbstractBuffEvent> list1))
                {
                    list1.Add(bf);
                }
                else
                {
                    _buffDataByDst[bf.To] = new List<AbstractBuffEvent>()
                    {
                        bf
                    };
                }
                buffAgentsToSort.Add(bf.To);
                if (_buffData.TryGetValue(bf.BuffID, out List<AbstractBuffEvent> list2))
                {
                    list2.Add(bf);
                }
                else
                {
                    _buffData[bf.BuffID] = new List<AbstractBuffEvent>()
                    {
                        bf
                    };
                }
                buffIDsToSort.Add(bf.BuffID);
            }
            foreach (long buffID in buffIDsToSort)
            {
                _buffData[buffID].Sort((x, y) => x.Time.CompareTo(y.Time));
            }
            foreach (AgentItem a in buffAgentsToSort)
            {
                _buffDataByDst[a].Sort((x, y) => x.Time.CompareTo(y.Time));
            }
        }

        private void EIDamageParse(SkillData skillData, FightData fightData)
        {
            var toAdd = new List<AbstractDamageEvent>();
            toAdd.AddRange(fightData.Logic.SpecialDamageEventProcess(_damageData, _damageTakenData, _damageDataById, skillData));
            var idsToSort = new HashSet<long>();
            var dstToSort = new HashSet<AgentItem>();
            var srcToSort = new HashSet<AgentItem>();
            foreach (AbstractDamageEvent de in toAdd)
            {
                if (_damageTakenData.TryGetValue(de.To, out List<AbstractDamageEvent> list1))
                {
                    list1.Add(de);
                }
                else
                {
                    _damageTakenData[de.To] = new List<AbstractDamageEvent>()
                    {
                        de
                    };
                }
                dstToSort.Add(de.To);
                if (_damageData.TryGetValue(de.From, out List<AbstractDamageEvent> list3))
                {
                    list1.Add(de);
                }
                else
                {
                    _damageData[de.From] = new List<AbstractDamageEvent>()
                    {
                        de
                    };
                }
                srcToSort.Add(de.To);
                if (_damageDataById.TryGetValue(de.SkillId, out List<AbstractDamageEvent> list2))
                {
                    list2.Add(de);
                }
                else
                {
                    _damageDataById[de.SkillId] = new List<AbstractDamageEvent>()
                    {
                        de
                    };
                }
                idsToSort.Add(de.SkillId);
            }
            foreach (long buffID in idsToSort)
            {
                _damageDataById[buffID].Sort((x, y) => x.Time.CompareTo(y.Time));
            }
            foreach (AgentItem a in dstToSort)
            {
                _damageTakenData[a].Sort((x, y) => x.Time.CompareTo(y.Time));
            }
            foreach (AgentItem a in srcToSort)
            {
                _damageData[a].Sort((x, y) => x.Time.CompareTo(y.Time));
            }
        }
        private void EICastParse(List<Player> players, SkillData skillData)
        {
            var toAdd = new List<AnimatedCastEvent>();
            foreach (Player p in players)
            {
                if (p.Prof == "Mirage")
                {
                    toAdd = MirageHelper.TranslateMirageCloak(GetBuffData(40408), skillData);
                    break;
                }
            }
            var castIDsToSort = new HashSet<long>();
            var castAgentsToSort = new HashSet<AgentItem>();
            foreach (AnimatedCastEvent cast in toAdd)
            {
                if (_animatedCastData.TryGetValue(cast.Caster, out List<AnimatedCastEvent> list1))
                {
                    list1.Add(cast);
                }
                else
                {
                    _animatedCastData[cast.Caster] = new List<AnimatedCastEvent>()
                    {
                        cast
                    };
                }
                castAgentsToSort.Add(cast.Caster);
                if (_castDataById.TryGetValue(cast.SkillId, out List<AbstractCastEvent> list2))
                {
                    list2.Add(cast);
                }
                else
                {
                    _castDataById[cast.SkillId] = new List<AbstractCastEvent>()
                    {
                        cast
                    };
                }
                castIDsToSort.Add(cast.SkillId);
            }
            foreach (long buffID in castIDsToSort)
            {
                _castDataById[buffID].Sort((x, y) => x.Time.CompareTo(y.Time));
            }
            foreach (AgentItem a in castAgentsToSort)
            {
                _animatedCastData[a].Sort((x, y) => x.Time.CompareTo(y.Time));
            }
        }

        private void EIStatusParse()
        {
            foreach (KeyValuePair<AgentItem, List<AbstractDamageEvent>> pair in _damageTakenData)
            {
                if (pair.Key.ID == (int)ArcDPSEnums.TargetID.WorldVersusWorld)
                {
                    continue;
                }
                bool setDeads = false;
                if (!_statusEvents.DeadEvents.TryGetValue(pair.Key, out List<DeadEvent> agentDeaths))
                {
                    agentDeaths = new List<DeadEvent>();
                    setDeads = true;
                }
                bool setDowns = false;
                if (!_statusEvents.DownEvents.TryGetValue(pair.Key, out List<DownEvent> agentDowns))
                {
                    agentDowns = new List<DownEvent>();
                    setDowns = true;
                }
                foreach (AbstractDamageEvent evt in pair.Value)
                {
                    if (evt.HasKilled)
                    {
                        if (!agentDeaths.Exists(x => Math.Abs(x.Time - evt.Time) < 500)) {
                            agentDeaths.Add(new DeadEvent(pair.Key, evt.Time));
                        }
                    }
                    if (evt.HasDowned)
                    {
                        if (!agentDowns.Exists(x => Math.Abs(x.Time - evt.Time) < 500))
                        {
                            agentDowns.Add(new DownEvent(pair.Key, evt.Time));
                        }
                    }
                }
                agentDowns.Sort((x,y) => x.Time.CompareTo(y.Time));
                agentDeaths.Sort((x, y) => x.Time.CompareTo(y.Time));
                if (setDeads && agentDeaths.Count > 0)
                {
                    _statusEvents.DeadEvents[pair.Key] = agentDeaths;
                }
                if (setDowns && agentDowns.Count > 0)
                {
                    _statusEvents.DownEvents[pair.Key] = agentDowns;
                }
            }
        }

        private void EIExtraEventProcess(List<Player> players, SkillData skillData, FightData fightData, OperationTracer operation)
        {
            operation.UpdateProgressWithCancellationCheck("Creating Custom Buff Events");
            EIBuffParse(players, skillData, fightData);
            operation.UpdateProgressWithCancellationCheck("Creating Custom Damage Events");
            EIDamageParse(skillData, fightData);
            operation.UpdateProgressWithCancellationCheck("Creating Custom Cast Events");
            EICastParse(players, skillData);
            operation.UpdateProgressWithCancellationCheck("Creating Custom Status Events");
            EIStatusParse();
            // master attachements
            operation.UpdateProgressWithCancellationCheck("Attaching Banners to Warriors");
            WarriorHelper.AttachMasterToWarriorBanners(players, _buffData, _castDataById);
            operation.UpdateProgressWithCancellationCheck("Attaching Turrets to Engineers");
            EngineerHelper.AttachMasterToEngineerTurrets(players, _damageDataById, _castDataById);
            operation.UpdateProgressWithCancellationCheck("Attaching Ranger Gadgets to Rangers");
            RangerHelper.AttachMasterToRangerGadgets(players, _damageDataById, _castDataById);
            operation.UpdateProgressWithCancellationCheck("Attaching Racial Gadgets to Players");
            ProfHelper.AttachMasterToRacialGadgets(players, _damageDataById, _castDataById);
        }

        internal CombatData(List<CombatItem> allCombatItems, FightData fightData, AgentData agentData, SkillData skillData, List<Player> players, OperationTracer operation)
        {
            _skillIds = new HashSet<long>(allCombatItems.Select(x => (long)x.SkillID));
            IEnumerable<CombatItem> noStateActiBuffRem = allCombatItems.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.None && x.IsActivation == ArcDPSEnums.Activation.None && x.IsBuffRemove == ArcDPSEnums.BuffRemove.None);
            // movement events
            _movementData = CombatEventFactory.CreateMovementEvents(allCombatItems.Where(x =>
                       x.IsStateChange == ArcDPSEnums.StateChange.Position ||
                       x.IsStateChange == ArcDPSEnums.StateChange.Velocity ||
                       x.IsStateChange == ArcDPSEnums.StateChange.Rotation).ToList(), agentData);
            HasMovementData = _movementData.Count > 1;
            // state change events
            operation.UpdateProgressWithCancellationCheck("Creatint status and metadata events");
            CombatEventFactory.CreateStateChangeEvents(allCombatItems, _metaDataEvents, _statusEvents, _rewardEvents, agentData);
            operation.UpdateProgressWithCancellationCheck("Combining SkillInfo with SkillData");
            skillData.CombineWithSkillInfo(_metaDataEvents.SkillInfoEvents);
            // activation events
            operation.UpdateProgressWithCancellationCheck("Creating Cast Events");
            List<AnimatedCastEvent> animatedCastData = CombatEventFactory.CreateCastEvents(allCombatItems.Where(x => x.IsActivation != ArcDPSEnums.Activation.None).ToList(), agentData, skillData);
            operation.UpdateProgressWithCancellationCheck("Creating Weapon Swap Events");
            List<WeaponSwapEvent> wepSwaps = CombatEventFactory.CreateWeaponSwapEvents(allCombatItems.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.WeaponSwap).ToList(), agentData, skillData);
            _weaponSwapData = wepSwaps.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
            _animatedCastData = animatedCastData.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
            var allCastEvents = new List<AbstractCastEvent>(animatedCastData);
            allCastEvents.AddRange(wepSwaps);
            _castDataById = allCastEvents.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            // buff remove event
            var buffCombatEvents = allCombatItems.Where(x => x.IsBuffRemove != ArcDPSEnums.BuffRemove.None && x.IsBuff != 0).ToList();
            buffCombatEvents.AddRange(noStateActiBuffRem.Where(x => x.IsBuff != 0 && x.BuffDmg == 0 && x.Value > 0));
            buffCombatEvents.AddRange(allCombatItems.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.BuffInitial));
            buffCombatEvents.Sort((x, y) => x.Time.CompareTo(y.Time));
            operation.UpdateProgressWithCancellationCheck("Creating Buff Events");
            List<AbstractBuffEvent> buffEvents = CombatEventFactory.CreateBuffEvents(buffCombatEvents, agentData, skillData);
            _buffDataByDst = buffEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _buffData = buffEvents.GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
            // damage events
            operation.UpdateProgressWithCancellationCheck("Creating Damage Events");
            List<AbstractDamageEvent> damageData = CombatEventFactory.CreateDamageEvents(noStateActiBuffRem.Where(x => (x.IsBuff != 0 && x.Value == 0) || (x.IsBuff == 0)).ToList(), agentData, skillData);
            _damageData = damageData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            _damageTakenData = damageData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _damageDataById = damageData.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());

            /*healing_data = allCombatItems.Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                         ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                         (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            healing_received_data = allCombatItems.Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();*/
            EIExtraEventProcess(players, skillData, fightData, operation);
            _buffRemoveAllData = _buffData.ToDictionary(x => x.Key, x => x.Value.OfType<BuffRemoveAllEvent>().ToList());
        }

        // getters

        public HashSet<long> GetSkills()
        {
            return _skillIds;
        }

        public List<AliveEvent> GetAliveEvents(AgentItem key)
        {
            if (_statusEvents.AliveEvents.TryGetValue(key, out List<AliveEvent> list))
            {
                return list;
            }
            return new List<AliveEvent>();
        }

        public List<AttackTargetEvent> GetAttackTargetEvents(AgentItem key)
        {
            if (_statusEvents.AttackTargetEvents.TryGetValue(key, out List<AttackTargetEvent> list))
            {
                return list;
            }
            return new List<AttackTargetEvent>();
        }

        public List<DeadEvent> GetDeadEvents(AgentItem key)
        {
            if (_statusEvents.DeadEvents.TryGetValue(key, out List<DeadEvent> list))
            {
                return list;
            }
            return new List<DeadEvent>();
        }

        public List<DespawnEvent> GetDespawnEvents(AgentItem key)
        {
            if (_statusEvents.DespawnEvents.TryGetValue(key, out List<DespawnEvent> list))
            {
                return list;
            }
            return new List<DespawnEvent>();
        }

        public List<DownEvent> GetDownEvents(AgentItem key)
        {
            if (_statusEvents.DownEvents.TryGetValue(key, out List<DownEvent> list))
            {
                return list;
            }
            return new List<DownEvent>();
        }

        public List<EnterCombatEvent> GetEnterCombatEvents(AgentItem key)
        {
            if (_statusEvents.EnterCombatEvents.TryGetValue(key, out List<EnterCombatEvent> list))
            {
                return list;
            }
            return new List<EnterCombatEvent>();
        }

        public List<ExitCombatEvent> GetExitCombatEvents(AgentItem key)
        {
            if (_statusEvents.ExitCombatEvents.TryGetValue(key, out List<ExitCombatEvent> list))
            {
                return list;
            }
            return new List<ExitCombatEvent>();
        }

        public List<GuildEvent> GetGuildEvents(AgentItem key)
        {
            if (_metaDataEvents.GuildEvents.TryGetValue(key, out List<GuildEvent> list))
            {
                return list;
            }
            return new List<GuildEvent>();
        }

        public List<HealthUpdateEvent> GetHealthUpdateEvents(AgentItem key)
        {
            if (_statusEvents.HealthUpdateEvents.TryGetValue(key, out List<HealthUpdateEvent> list))
            {
                return list;
            }
            return new List<HealthUpdateEvent>();
        }

        public List<MaxHealthUpdateEvent> GetMaxHealthUpdateEvents(AgentItem key)
        {
            if (_statusEvents.MaxHealthUpdateEvents.TryGetValue(key, out List<MaxHealthUpdateEvent> list))
            {
                return list;
            }
            return new List<MaxHealthUpdateEvent>();
        }

        public PointOfViewEvent GetPointOfViewEvent()
        {
            return _metaDataEvents.PointOfViewEvent;
        }

        public List<SpawnEvent> GetSpawnEvents(AgentItem key)
        {
            if (_statusEvents.SpawnEvents.TryGetValue(key, out List<SpawnEvent> list))
            {
                return list;
            }
            return new List<SpawnEvent>();
        }

        public List<TargetableEvent> GetTargetableEvents(AgentItem key)
        {
            if (_statusEvents.TargetableEvents.TryGetValue(key, out List<TargetableEvent> list))
            {
                return list;
            }
            return new List<TargetableEvent>();
        }

        /*public List<TagEvent> GetTagEvents(AgentItem key)
        {
            if (_statusEvents.TagEvents.TryGetValue(key, out List<TagEvent> list))
            {
                return list;
            }
            return new List<TagEvent>();
        }*/

        public List<TeamChangeEvent> GetTeamChangeEvents(AgentItem key)
        {
            if (_statusEvents.TeamChangeEvents.TryGetValue(key, out List<TeamChangeEvent> list))
            {
                return list;
            }
            return new List<TeamChangeEvent>();
        }

        public List<BreakbarStateEvent> GetBreakbarStateEvents(AgentItem key)
        {
            if (_statusEvents.BreakbarStateEvents.TryGetValue(key, out List<BreakbarStateEvent> list))
            {
                return list;
            }
            return new List<BreakbarStateEvent>();
        }

        public List<BreakbarPercentEvent> GetBreakbarPercentEvents(AgentItem key)
        {
            if (_statusEvents.BreakbarPercentEvents.TryGetValue(key, out List<BreakbarPercentEvent> list))
            {
                return list;
            }
            return new List<BreakbarPercentEvent>();
        }

        public BuildEvent GetBuildEvent()
        {
            return _metaDataEvents.BuildEvent;
        }

        public LanguageEvent GetLanguageEvent()
        {
            return _metaDataEvents.LanguageEvent;
        }

        public LogStartEvent GetLogStartEvent()
        {
            return _metaDataEvents.LogStartEvent;
        }

        public LogEndEvent GetLogEndEvent()
        {
            return _metaDataEvents.LogEndEvent;
        }

        public List<MapIDEvent> GetMapIDEvents()
        {
            return _metaDataEvents.MapIDEvents;
        }

        public List<RewardEvent> GetRewardEvents()
        {
            return _rewardEvents;
        }

        public List<ErrorEvent> GetErrorEvents()
        {
            return _metaDataEvents.ErrorEvents;
        }

        public List<ShardEvent> GetShardEvents()
        {
            return _metaDataEvents.ShardEvents;
        }

        public BuffInfoEvent GetBuffInfoEvent(long buffID)
        {
            if (_metaDataEvents.BuffInfoEvents.TryGetValue(buffID, out BuffInfoEvent evt))
            {
                return evt;
            }
            return null;
        }

        public List<BuffInfoEvent> GetBuffInfoEvent(ArcDPSEnums.BuffCategory category)
        {
            if (_metaDataEvents.BuffInfoEventsByCategory.TryGetValue(category, out List<BuffInfoEvent> evts))
            {
                return evts;
            }
            return new List<BuffInfoEvent>();
        }

        public SkillInfoEvent GetSkillInfoEvent(long skillID)
        {
            if (_metaDataEvents.SkillInfoEvents.TryGetValue(skillID, out SkillInfoEvent evt))
            {
                return evt;
            }
            return null;
        }

        public List<AbstractBuffEvent> GetBuffData(long key)
        {
            if (_buffData.TryGetValue(key, out List<AbstractBuffEvent> res))
            {
                return res;
            }
            return new List<AbstractBuffEvent>(); ;
        }

        public List<BuffRemoveAllEvent> GetBuffRemoveAllData(long key)
        {
            if (_buffRemoveAllData.TryGetValue(key, out List<BuffRemoveAllEvent> res))
            {
                return res;
            }
            return new List<BuffRemoveAllEvent>(); ;
        }

        /// <summary>
        /// Returns list of buff events applied on agent
        /// </summary>
        /// <param name="key"></param> Agent
        /// <returns></returns>
        public List<AbstractBuffEvent> GetBuffData(AgentItem key)
        {
            if (_buffDataByDst.TryGetValue(key, out List<AbstractBuffEvent> res))
            {
                return res;
            }
            return new List<AbstractBuffEvent>(); ;
        }

        /// <summary>
        /// Returns list of damage events done by agent
        /// </summary>
        /// <param name="key"></param> Agent
        /// <returns></returns>
        public List<AbstractDamageEvent> GetDamageData(AgentItem key)
        {
            if (_damageData.TryGetValue(key, out List<AbstractDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractDamageEvent>(); ;
        }

        /// <summary>
        /// Returns list of damage events applied by a skill
        /// </summary>
        /// <param name="key"></param> Id of the skill
        /// <returns></returns>
        public List<AbstractDamageEvent> GetDamageData(long key)
        {
            if (_damageDataById.TryGetValue(key, out List<AbstractDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractDamageEvent>(); ;
        }

        /// <summary>
        /// Returns list of animated cast events done by Agent
        /// </summary>
        /// <param name="key"></param> Agent
        /// <returns></returns>
        public List<AnimatedCastEvent> GetAnimatedCastData(AgentItem key)
        {
            if (_animatedCastData.TryGetValue(key, out List<AnimatedCastEvent> res))
            {
                return res;
            }
            return new List<AnimatedCastEvent>(); ;
        }

        /// <summary>
        /// Returns list of weapon swap events done by Agent
        /// </summary>
        /// <param name="key"></param> Agent
        /// <returns></returns>
        public List<WeaponSwapEvent> GetWeaponSwapData(AgentItem key)
        {
            if (_weaponSwapData.TryGetValue(key, out List<WeaponSwapEvent> res))
            {
                return res;
            }
            return new List<WeaponSwapEvent>(); ;
        }

        /// <summary>
        /// Returns list of cast events from skill
        /// </summary>
        /// <param name="key"></param> ID of the skill
        /// <returns></returns>
        public List<AbstractCastEvent> GetCastData(long key)
        {
            if (_castDataById.TryGetValue(key, out List<AbstractCastEvent> res))
            {
                return res;
            }
            return new List<AbstractCastEvent>(); ;
        }

        /// <summary>
        /// Returns list of damage taken events by Agent
        /// </summary>
        /// <param name="key"></param> Agent
        /// <returns></returns>
        public List<AbstractDamageEvent> GetDamageTakenData(AgentItem key)
        {
            if (_damageTakenData.TryGetValue(key, out List<AbstractDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractDamageEvent>();
        }

        /*public List<CombatItem> getHealingData()
        {
            return _healingData;
        }

        public List<CombatItem> getHealingReceivedData()
        {
            return _healingReceivedData;
        }*/


        public List<AbstractMovementEvent> GetMovementData(AgentItem key)
        {
            if (_movementData.TryGetValue(key, out List<AbstractMovementEvent> res))
            {
                return res;
            }
            return new List<AbstractMovementEvent>();
        }

    }
}
