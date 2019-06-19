﻿using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
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
        private readonly Dictionary<AgentItem, List<AnimatedCastEvent>> _castData;
        private readonly Dictionary<AgentItem, List<WeaponSwapEvent>> _weaponSwapData;
        private readonly Dictionary<long, List<AbstractCastEvent>> _castDataById;
        private readonly Dictionary<AgentItem, List<AbstractDamageEvent>> _damageTakenData;
        private readonly Dictionary<AgentItem, List<AbstractMovementEvent>> _movementData;

        private void SpecialBoonParse(List<Player> players, SkillData skillData, FightData fightData)
        {
            List<AbstractBuffEvent> toAdd = new List<AbstractBuffEvent>();
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
            toAdd.AddRange(fightData.Logic.CreateCustomBuffEvents(_boonDataByDst, _boonData, fightData, skillData));
            HashSet<long> buffIDsToSort = new HashSet<long>();
            HashSet<AgentItem> buffAgentsToSort = new HashSet<AgentItem>();
            foreach (AbstractBuffEvent bf in toAdd)
            {
                _boonDataByDst[bf.To].Add(bf);
                if (_boonDataByDst.TryGetValue(bf.To, out var list1))
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
                if (_boonData.TryGetValue(bf.BuffID, out var list2))
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

        private void SpecialCastParse(List<Player> players, SkillData skillData)
        {
            List<AnimatedCastEvent> toAdd = new List<AnimatedCastEvent>();
            foreach (Player p in players)
            {
                if (p.Prof == "Mirage")
                {
                    toAdd = MirageHelper.TranslateMirageCloak(GetBoonData(40408), skillData);
                    break;
                }
            }
            HashSet<long> castIDsToSort = new HashSet<long>();
            HashSet<AgentItem> castAgentsToSort = new HashSet<AgentItem>();
            foreach (AnimatedCastEvent cast in toAdd)
            {
                if (_castData.TryGetValue(cast.Caster, out var list1))
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
                if (_castDataById.TryGetValue(cast.SkillId, out var list2))
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
            SpecialCastParse(players, skillData);
        }

        public CombatData(List<CombatItem> allCombatItems, FightData fightData, AgentData agentData, SkillData skillData, List<Player> players)
        {
            _skillIds = new HashSet<long>(allCombatItems.Select(x => x.SkillID));
            var noStateActiBuffRem = allCombatItems.Where(x => x.IsStateChange == ParseEnum.StateChange.None && x.IsActivation == ParseEnum.Activation.None && x.IsBuffRemove == ParseEnum.BuffRemove.None);
            // movement events
            _movementData = EICombatEventFactory.CreateMovementEvents(allCombatItems.Where(x =>
                       x.IsStateChange == ParseEnum.StateChange.Position ||
                       x.IsStateChange == ParseEnum.StateChange.Velocity ||
                       x.IsStateChange == ParseEnum.StateChange.Rotation).ToList(), agentData, fightData.FightStartLogTime);
            HasMovementData = _movementData.Count > 1;
            // state change events
            EICombatEventFactory.CreateStateChangeEvents(allCombatItems, _metaDataEvents, _statusEvents, agentData, fightData.FightStartLogTime);
            // activation events
            List<AnimatedCastEvent> castData = EICombatEventFactory.CreateCastEvents(allCombatItems.Where(x => x.IsActivation != ParseEnum.Activation.None).ToList(), agentData, skillData, fightData.FightStartLogTime);
            List<WeaponSwapEvent> wepSwaps = EICombatEventFactory.CreateWeaponSwapEvents(allCombatItems.Where(x => x.IsStateChange == ParseEnum.StateChange.WeaponSwap).ToList(), agentData, skillData, fightData.FightStartLogTime);
            _weaponSwapData = wepSwaps.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
            _castData = castData.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
            List<AbstractCastEvent> allCastEvents = new List<AbstractCastEvent>(castData);
            allCastEvents.AddRange(wepSwaps);
            _castDataById = allCastEvents.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            // buff remove event
            List<CombatItem> buffCombatEvents = allCombatItems.Where(x => x.IsBuffRemove != ParseEnum.BuffRemove.None && x.IsBuff != 0).ToList();
            buffCombatEvents.AddRange(noStateActiBuffRem.Where(x => x.IsBuff != 0 && x.BuffDmg == 0 && x.Value > 0));
            buffCombatEvents.AddRange(allCombatItems.Where(x => x.IsStateChange == ParseEnum.StateChange.BuffInitial));
            buffCombatEvents.Sort((x, y) => x.LogTime.CompareTo(y.LogTime));
            List<AbstractBuffEvent> buffEvents = EICombatEventFactory.CreateBuffEvents(buffCombatEvents, agentData, skillData, fightData.FightStartLogTime);
            _boonDataByDst = buffEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _boonData = buffEvents.GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
            // damage events
            List<AbstractDamageEvent> damageData = EICombatEventFactory.CreateDamageEvents(noStateActiBuffRem.Where(x => (x.IsBuff != 0 && x.Value == 0) || (x.IsBuff == 0)).ToList(), agentData, skillData, fightData.FightStartLogTime);
            _damageData = damageData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            _damageTakenData = damageData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());

            /*healing_data = allCombatItems.Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                         ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                         (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            healing_received_data = allCombatItems.Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();*/
            ExtraEvents(players, skillData, fightData);
        }

        public void UpdateDamageEvents(long end)
        {
            List<AbstractDamageEvent> damageData = _damageData.SelectMany(x => x.Value).ToList();
            damageData.Sort((x, y) => x.Time.CompareTo(y.Time));
            damageData.Reverse();
            foreach (AbstractDamageEvent c in damageData)
            {
                if (c.Time <= end)
                {
                    break;
                }
                else if (c.Time <= end + 1000)
                {
                    c.OverrideTime(end);
                }
            }
        }

        // getters

        public HashSet<long> GetSkills()
        {
            return _skillIds;
        }

        public List<AliveEvent> GetAliveEvents(AgentItem key)
        {
            if (_statusEvents.AliveEvents.TryGetValue(key, out var list))
            {
                return list;
            }
            return new List<AliveEvent>();
        }

        public List<AttackTargetEvent> GetAttackTargetEvents(AgentItem key)
        {
            if (_statusEvents.AttackTargetEvents.TryGetValue(key, out var list))
            {
                return list;
            }
            return new List<AttackTargetEvent>();
        }

        public List<DeadEvent> GetDeadEvents(AgentItem key)
        {
            if (_statusEvents.DeadEvents.TryGetValue(key, out var list))
            {
                return list;
            }
            return new List<DeadEvent>();
        }

        public List<DespawnEvent> GetDespawnEvents(AgentItem key)
        {
            if (_statusEvents.DespawnEvents.TryGetValue(key, out var list))
            {
                return list;
            }
            return new List<DespawnEvent>();
        }

        public List<DownEvent> GetDownEvents(AgentItem key)
        {
            if (_statusEvents.DownEvents.TryGetValue(key, out var list))
            {
                return list;
            }
            return new List<DownEvent>();
        }

        public List<EnterCombatEvent> GetEnterCombatEvents(AgentItem key)
        {
            if (_statusEvents.EnterCombatEvents.TryGetValue(key, out var list))
            {
                return list;
            }
            return new List<EnterCombatEvent>();
        }

        public List<ExitCombatEvent> GetExitCombatEvents(AgentItem key)
        {
            if (_statusEvents.ExitCombatEvents.TryGetValue(key, out var list))
            {
                return list;
            }
            return new List<ExitCombatEvent>();
        }

        public List<GuildEvent> GetGuildEvents(AgentItem key)
        {
            if (_statusEvents.GuildEvents.TryGetValue(key, out var list))
            {
                return list;
            }
            return new List<GuildEvent>();
        }

        public List<HealthUpdateEvent> GetHealthUpdateEvents(AgentItem key)
        {
            if (_statusEvents.HealthUpdateEvents.TryGetValue(key, out var list))
            {
                return list;
            }
            return new List<HealthUpdateEvent>();
        }

        public List<MaxHealthUpdateEvent> GetMaxHealthUpdateEvents(AgentItem key)
        {
            if (_statusEvents.MaxHealthUpdateEvents.TryGetValue(key, out var list))
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
            if (_statusEvents.SpawnEvents.TryGetValue(key, out var list))
            {
                return list;
            }
            return new List<SpawnEvent>();
        }

        public List<TargetableEvent> GetTargetableEvents(AgentItem key)
        {
            if (_statusEvents.TargetableEvents.TryGetValue(key, out var list))
            {
                return list;
            }
            return new List<TargetableEvent>();
        }

        public List<TeamChangeEvent> GetTeamChangeEvents(AgentItem key)
        {
            if (_statusEvents.TeamChangeEvents.TryGetValue(key, out var list))
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