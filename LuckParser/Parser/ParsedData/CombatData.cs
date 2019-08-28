using System.Collections.Generic;
using System.Linq;
using LuckParser.EIData;
using LuckParser.Parser.ParsedData.CombatEvents;

namespace LuckParser.Parser.ParsedData
{
    public class CombatData
    {
        public bool HasMovementData { get; }

        //private List<CombatItem> _healingData;
        //private List<CombatItem> _healingReceivedData;
        private readonly StatusEventsContainer _statusEvents = new StatusEventsContainer();
        private readonly MetaEventsContainer _metaDataEvents = new MetaEventsContainer();
        private readonly HashSet<long> _skillIds;
        private readonly Dictionary<long, List<AbstractBuffEvent>> _boonData;
        private readonly Dictionary<AgentItem, List<AbstractBuffEvent>> _boonDataByDst;
        private readonly Dictionary<AgentItem, List<AbstractDamageEvent>> _damageData;
        private readonly Dictionary<long, List<AbstractDamageEvent>> _damageDataById;
        private readonly Dictionary<AgentItem, List<AnimatedCastEvent>> _castData;
        private readonly Dictionary<AgentItem, List<WeaponSwapEvent>> _weaponSwapData;
        private readonly Dictionary<long, List<AbstractCastEvent>> _castDataById;
        private readonly Dictionary<AgentItem, List<AbstractDamageEvent>> _damageTakenData;
        private readonly Dictionary<AgentItem, List<AbstractMovementEvent>> _movementData;

        private void SpecialBoonParse(List<Player> players, SkillData skillData, FightData fightData)
        {
            var toAdd = new List<AbstractBuffEvent>();
            foreach (Player p in players)
            {
                if (p.Prof == "Weaver")
                {
                    toAdd = WeaverHelper.TransformWeaverAttunements(GetBoonDataByDst(p.AgentItem), p.AgentItem, skillData);
                }
                if (p.Prof == "Elementalist" || p.Prof == "Tempest")
                {
                    ElementalistHelper.RemoveDualBuffs(GetBoonDataByDst(p.AgentItem), skillData);
                }
            }
            toAdd.AddRange(fightData.Logic.SpecialBuffEventProcess(_boonDataByDst, _boonData, fightData.FightStartLogTime, skillData));
            var buffIDsToSort = new HashSet<long>();
            var buffAgentsToSort = new HashSet<AgentItem>();
            foreach (AbstractBuffEvent bf in toAdd)
            {
                if (_boonDataByDst.TryGetValue(bf.To, out List<AbstractBuffEvent> list1))
                {
                    list1.Add(bf);
                }
                else
                {
                    _boonDataByDst[bf.To] = new List<AbstractBuffEvent>()
                    {
                        bf
                    };
                }
                buffAgentsToSort.Add(bf.To);
                if (_boonData.TryGetValue(bf.BuffID, out List<AbstractBuffEvent> list2))
                {
                    list2.Add(bf);
                }
                else
                {
                    _boonData[bf.BuffID] = new List<AbstractBuffEvent>()
                    {
                        bf
                    };
                }
                buffIDsToSort.Add(bf.BuffID);
            }
            foreach (long buffID in buffIDsToSort)
            {
                _boonData[buffID].Sort((x, y) => x.Time.CompareTo(y.Time));
            }
            foreach (AgentItem a in buffAgentsToSort)
            {
                _boonDataByDst[a].Sort((x, y) => x.Time.CompareTo(y.Time));
            }
        }

        private void SpecialDamageParse(SkillData skillData, FightData fightData)
        {
            var toAdd = new List<AbstractDamageEvent>();
            toAdd.AddRange(fightData.Logic.SpecialDamageEventProcess(_damageData, _damageTakenData, _damageDataById, fightData.FightStartLogTime, skillData));
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
        private void SpecialCastParse(List<Player> players, SkillData skillData)
        {
            var toAdd = new List<AnimatedCastEvent>();
            foreach (Player p in players)
            {
                if (p.Prof == "Mirage")
                {
                    toAdd = MirageHelper.TranslateMirageCloak(GetBoonData(40408), skillData);
                    break;
                }
            }
            var castIDsToSort = new HashSet<long>();
            var castAgentsToSort = new HashSet<AgentItem>();
            foreach (AnimatedCastEvent cast in toAdd)
            {
                if (_castData.TryGetValue(cast.Caster, out List<AnimatedCastEvent> list1))
                {
                    list1.Add(cast);
                }
                else
                {
                    _castData[cast.Caster] = new List<AnimatedCastEvent>()
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
                _castData[a].Sort((x, y) => x.Time.CompareTo(y.Time));
            }
        }

        private void ExtraEvents(List<Player> players, SkillData skillData, FightData fightData)
        {
            SpecialBoonParse(players, skillData, fightData);
            SpecialDamageParse(skillData, fightData);
            SpecialCastParse(players, skillData);
        }

        public CombatData(List<CombatItem> allCombatItems, FightData fightData, AgentData agentData, SkillData skillData, List<Player> players)
        {
            _skillIds = new HashSet<long>(allCombatItems.Select(x => x.SkillID));
            IEnumerable<CombatItem> noStateActiBuffRem = allCombatItems.Where(x => x.IsStateChange == ParseEnum.StateChange.None && x.IsActivation == ParseEnum.Activation.None && x.IsBuffRemove == ParseEnum.BuffRemove.None);
            // movement events
            _movementData = CombatEventFactory.CreateMovementEvents(allCombatItems.Where(x =>
                       x.IsStateChange == ParseEnum.StateChange.Position ||
                       x.IsStateChange == ParseEnum.StateChange.Velocity ||
                       x.IsStateChange == ParseEnum.StateChange.Rotation).ToList(), agentData, fightData.FightStartLogTime);
            HasMovementData = _movementData.Count > 1;
            // state change events
            CombatEventFactory.CreateStateChangeEvents(allCombatItems, _metaDataEvents, _statusEvents, agentData, fightData.FightStartLogTime);
            // activation events
            List<AnimatedCastEvent> castData = CombatEventFactory.CreateCastEvents(allCombatItems.Where(x => x.IsActivation != ParseEnum.Activation.None).ToList(), agentData, skillData, fightData.FightStartLogTime);
            List<WeaponSwapEvent> wepSwaps = CombatEventFactory.CreateWeaponSwapEvents(allCombatItems.Where(x => x.IsStateChange == ParseEnum.StateChange.WeaponSwap).ToList(), agentData, skillData, fightData.FightStartLogTime);
            _weaponSwapData = wepSwaps.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
            _castData = castData.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
            var allCastEvents = new List<AbstractCastEvent>(castData);
            allCastEvents.AddRange(wepSwaps);
            _castDataById = allCastEvents.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            // buff remove event
            var buffCombatEvents = allCombatItems.Where(x => x.IsBuffRemove != ParseEnum.BuffRemove.None && x.IsBuff != 0).ToList();
            buffCombatEvents.AddRange(noStateActiBuffRem.Where(x => x.IsBuff != 0 && x.BuffDmg == 0 && x.Value > 0));
            buffCombatEvents.AddRange(allCombatItems.Where(x => x.IsStateChange == ParseEnum.StateChange.BuffInitial));
            buffCombatEvents.Sort((x, y) => x.LogTime.CompareTo(y.LogTime));
            List<AbstractBuffEvent> buffEvents = CombatEventFactory.CreateBuffEvents(buffCombatEvents, agentData, skillData, fightData.FightStartLogTime);
            _boonDataByDst = buffEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _boonData = buffEvents.GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
            // damage events
            List<AbstractDamageEvent> damageData = CombatEventFactory.CreateDamageEvents(noStateActiBuffRem.Where(x => (x.IsBuff != 0 && x.Value == 0) || (x.IsBuff == 0)).ToList(), agentData, skillData, fightData.FightStartLogTime);
            _damageData = damageData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            _damageTakenData = damageData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _damageDataById = damageData.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());

            /*healing_data = allCombatItems.Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                         ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                         (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            healing_received_data = allCombatItems.Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();*/
            ExtraEvents(players, skillData, fightData);
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
            if (_statusEvents.GuildEvents.TryGetValue(key, out List<GuildEvent> list))
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

        public List<PointOfViewEvent> GetPointOfViewEvents()
        {
            return _metaDataEvents.PointOfViewEvents;
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

        public List<TeamChangeEvent> GetTeamChangeEvents(AgentItem key)
        {
            if (_statusEvents.TeamChangeEvents.TryGetValue(key, out List<TeamChangeEvent> list))
            {
                return list;
            }
            return new List<TeamChangeEvent>();
        }

        public List<BuildEvent> GetBuildEvents()
        {
            return _metaDataEvents.BuildEvents;
        }

        public List<LanguageEvent> GetLanguageEvents()
        {
            return _metaDataEvents.LanguageEvents;
        }

        public List<LogStartEvent> GetLogStartEvents()
        {
            return _metaDataEvents.LogStartEvents;
        }

        public List<LogEndEvent> GetLogEndEvents()
        {
            return _metaDataEvents.LogEndEvents;
        }

        public List<MapIDEvent> GetMapIDEvents()
        {
            return _metaDataEvents.MapIDEvents;
        }

        public List<RewardEvent> GetRewardEvents()
        {
            return _metaDataEvents.RewardEvents;
        }

        public List<ShardEvent> GetShardEvents()
        {
            return _metaDataEvents.ShardEvents;
        }

        public List<AbstractBuffEvent> GetBoonData(long key)
        {
            if (_boonData.TryGetValue(key, out List<AbstractBuffEvent> res))
            {
                return res;
            }
            return new List<AbstractBuffEvent>(); ;
        }

        public List<AbstractBuffEvent> GetBoonDataByDst(AgentItem key)
        {
            if (_boonDataByDst.TryGetValue(key, out List<AbstractBuffEvent> res))
            {
                return res;
            }
            return new List<AbstractBuffEvent>(); ;
        }


        public List<AbstractDamageEvent> GetDamageData(AgentItem key)
        {
            if (_damageData.TryGetValue(key, out List<AbstractDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractDamageEvent>(); ;
        }

        public List<AbstractDamageEvent> GetDamageDataById(long key)
        {
            if (_damageDataById.TryGetValue(key, out List<AbstractDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractDamageEvent>(); ;
        }

        public List<AnimatedCastEvent> GetCastData(AgentItem key)
        {
            if (_castData.TryGetValue(key, out List<AnimatedCastEvent> res))
            {
                return res;
            }
            return new List<AnimatedCastEvent>(); ;
        }

        public List<WeaponSwapEvent> GetWeaponSwapData(AgentItem key)
        {
            if (_weaponSwapData.TryGetValue(key, out List<WeaponSwapEvent> res))
            {
                return res;
            }
            return new List<WeaponSwapEvent>(); ;
        }


        public List<AbstractCastEvent> GetCastDataById(long key)
        {
            if (_castDataById.TryGetValue(key, out List<AbstractCastEvent> res))
            {
                return res;
            }
            return new List<AbstractCastEvent>(); ;
        }

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
