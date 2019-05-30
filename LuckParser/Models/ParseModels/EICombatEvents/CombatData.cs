using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class CombatData
    {
        public bool HasMovementData { get; }

        private Dictionary<ParseEnum.StateChange, List<CombatItem>> _statesData;
        //private List<CombatItem> _healingData;
        //private List<CombatItem> _healingReceivedData;
        private readonly HashSet<long> _skillIds;
        private readonly Dictionary<long, List<AbstractBuffEvent>> _boonData;
        private readonly Dictionary<AgentItem, List<AbstractBuffEvent>> _boonDataByDst;
        private readonly Dictionary<AgentItem, List<AbstractDamageEvent>> _damageData;
        private readonly Dictionary<AgentItem, List<AnimatedCastEvent>> _castData;
        private readonly Dictionary<AgentItem, List<WeaponSwapEvent>> _weaponSwapData;
        private readonly Dictionary<long, List<AbstractCastEvent>> _castDataById;
        private readonly Dictionary<AgentItem, List<AbstractDamageEvent>> _damageTakenData;
        private readonly Dictionary<AgentItem, List<AbstractMovementEvent>> _movementData;

        private void SpecialBoonParse(List<Player> players, List<AbstractBuffEvent> buffEvents)
        {
            bool resort = false;
            foreach (Player p in players)
            {
                if (p.Prof == "Weaver")
                {
                    List<AbstractBuffEvent> toAdd = WeaverHelper.TransformWeaverAttunements(buffEvents.Where(x => x.To == p.AgentItem).ToList(), p.AgentItem);
                    resort = resort || toAdd.Count > 0;
                    buffEvents.AddRange(toAdd);
                }
                if (p.Prof == "Elementalist" || p.Prof == "Tempest")
                {
                    ElementalistHelper.RemoveDualBuffs(buffEvents.Where(x => x.To == p.AgentItem).ToList());
                }
            }
            if (resort)
            {
                buffEvents.Sort((x, y) => x.Time.CompareTo(y.Time));
            }
        }

        public CombatData(List<CombatItem> allCombatItems, FightData fightData, AgentData agentData, List<Player> players)
        {
            _skillIds = new HashSet<long>(allCombatItems.Select(x => x.SkillID));
            var noStateActiBuffRem = allCombatItems.Where(x => x.IsStateChange == ParseEnum.StateChange.None && x.IsActivation == ParseEnum.Activation.None && x.IsBuffRemove == ParseEnum.BuffRemove.None);
            // movement events
            _movementData = fightData.Logic.HasCombatReplayMap
                ? EICombatEventFactory.CreateMovementEvents(allCombatItems.Where(x =>
                        x.IsStateChange == ParseEnum.StateChange.Position ||
                        x.IsStateChange == ParseEnum.StateChange.Velocity ||
                        x.IsStateChange == ParseEnum.StateChange.Rotation).ToList(), agentData, fightData.FightStart)
                : new Dictionary<AgentItem, List<AbstractMovementEvent>>();
            HasMovementData = _movementData.Count > 1;
            // state change events
            List<AbstractMetaDataEvent> metaDataEvents = new List<AbstractMetaDataEvent>();
            List<AbstractStatusEvent> statusEvents = new List<AbstractStatusEvent>();
            EICombatEventFactory.CreateStateChangeEvents(allCombatItems, metaDataEvents, statusEvents, agentData, fightData.FightStart);
            _statesData = allCombatItems.GroupBy(x => x.IsStateChange).ToDictionary(x => x.Key, x => x.ToList());
            // activation events
            List<AnimatedCastEvent> castData = EICombatEventFactory.CreateCastEvents(allCombatItems.Where(x => x.IsActivation != ParseEnum.Activation.None).ToList(), agentData, fightData.FightStart);
            List<WeaponSwapEvent> wepSwaps = EICombatEventFactory.CreateWeaponSwapEvents(allCombatItems.Where(x => x.IsStateChange == ParseEnum.StateChange.WeaponSwap).ToList(), agentData, fightData.FightStart);
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
            List<AbstractBuffEvent> buffEvents = EICombatEventFactory.CreateBuffEvents(buffCombatEvents, agentData, fightData.FightStart);
            SpecialBoonParse(players, buffEvents);
            _boonDataByDst = buffEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _boonData = buffEvents.GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
            // damage events
            List<AbstractDamageEvent> damageData = EICombatEventFactory.CreateDamageEvents(noStateActiBuffRem.Where(x => (x.IsBuff != 0 && x.Value == 0) || (x.IsBuff == 0)).ToList(), agentData, fightData.FightStart);
            _damageData = damageData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            _damageTakenData = damageData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());

            /*healing_data = allCombatItems.Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                         ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                         (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            healing_received_data = allCombatItems.Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();*/
        }

        public List<CombatItem> GetStatesData(ushort srcInstid, ParseEnum.StateChange change, long start, long end)
        {
            if (_statesData.TryGetValue(change, out List<CombatItem> data))
            {
                return data.Where(x => x.SrcInstid == srcInstid && x.LogTime >= start && x.LogTime <= end).ToList();
            }
            return new List<CombatItem>();
        }

        public void Update(long end)
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

        public List<CombatItem> GetStates(ParseEnum.StateChange key)
        {
            if (_statesData.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>();
        }


        public void GetAgentStatus(long start, long end, ushort instid, List<(long start, long end)> dead, List<(long start, long end)> down, List<(long start, long end)> dc, long fightStart, long fightEnd)
        {
            List<CombatItem> status = GetStatesData(instid, ParseEnum.StateChange.ChangeDown, start, end);
            status.AddRange(GetStatesData(instid, ParseEnum.StateChange.ChangeUp, start, end));
            status.AddRange(GetStatesData(instid, ParseEnum.StateChange.ChangeDead, start, end));
            status.AddRange(GetStatesData(instid, ParseEnum.StateChange.Spawn, start, end));
            status.AddRange(GetStatesData(instid, ParseEnum.StateChange.Despawn, start, end));
            status = status.OrderBy(x => x.LogTime).ToList();
            for (var i = 0; i < status.Count - 1; i++)
            {
                CombatItem cur = status[i];
                CombatItem next = status[i + 1];
                if (cur.IsStateChange == ParseEnum.StateChange.ChangeDown)
                {
                    down.Add((cur.LogTime - fightStart, next.LogTime - fightStart));
                }
                else if (cur.IsStateChange == ParseEnum.StateChange.ChangeDead)
                {
                    dead.Add((cur.LogTime - fightStart, next.LogTime - fightStart));
                }
                else if (cur.IsStateChange == ParseEnum.StateChange.Despawn)
                {
                    dc.Add((cur.LogTime - fightStart, next.LogTime - fightStart));
                }
            }
            // check last value
            if (status.Count > 0)
            {
                CombatItem cur = status.Last();
                if (cur.IsStateChange == ParseEnum.StateChange.ChangeDown)
                {
                    down.Add((cur.LogTime - fightStart, fightEnd - fightStart));
                }
                else if (cur.IsStateChange == ParseEnum.StateChange.ChangeDead)
                {
                    dead.Add((cur.LogTime - fightStart, fightEnd - fightStart));
                }
                else if (cur.IsStateChange == ParseEnum.StateChange.Despawn)
                {
                    dc.Add((cur.LogTime - fightStart, fightEnd - fightStart));
                }
            }
        }

    }
}