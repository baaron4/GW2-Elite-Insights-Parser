using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class CombatData
    {
        public List<CombatItem> AllCombatItems { get; }
        private Dictionary<ParseEnum.StateChange, List<CombatItem>> _statesData;
        //private List<CombatItem> _healingData;
        //private List<CombatItem> _healingReceivedData;
        private Dictionary<long, List<CombatItem>> _skillData;
        private Dictionary<long, List<CombatItem>> _boonData;
        private Dictionary<ushort, List<CombatItem>> _boonDataByDst;
        private Dictionary<ushort, List<CombatItem>> _damageData;
        private Dictionary<ushort, List<CombatItem>> _castData;
        private Dictionary<long, List<CombatItem>> _castDataById;
        private Dictionary<ushort, List<CombatItem>> _damageTakenData;
        private Dictionary<ushort, List<CombatItem>> _movementData;

        public CombatData(List<CombatItem> allCombatItems, FightData fightData)
        {
            AllCombatItems = allCombatItems;
            _skillData = allCombatItems.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
            var noStateActiBuffRem = allCombatItems.Where(x => x.IsStateChange == ParseEnum.StateChange.Normal && x.IsActivation == ParseEnum.Activation.None && x.IsBuffRemove == ParseEnum.BuffRemove.None);
            // movement events
            _movementData = fightData.Logic.CanCombatReplay
                ? allCombatItems.Where(x =>
                        x.IsStateChange == ParseEnum.StateChange.Position ||
                        x.IsStateChange == ParseEnum.StateChange.Velocity ||
                        x.IsStateChange == ParseEnum.StateChange.Rotation).GroupBy(x => x.SrcInstid)
                    .ToDictionary(x => x.Key, x => x.ToList())
                : new Dictionary<ushort, List<CombatItem>>();
            fightData.Logic.CanCombatReplay = _movementData.Count > 1;
            // state change events
            _statesData = allCombatItems.GroupBy(x => x.IsStateChange).ToDictionary(x => x.Key, x => x.ToList());
            // activation events
            var castData = allCombatItems.Where(x => x.IsActivation != ParseEnum.Activation.None || x.IsStateChange == ParseEnum.StateChange.WeaponSwap);
            _castData = castData.GroupBy(x => x.SrcInstid).ToDictionary(x => x.Key, x => x.ToList());
            _castDataById = castData.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
            // buff remove event
            var buffRemove = allCombatItems.Where(x => x.IsBuffRemove != ParseEnum.BuffRemove.None && x.IsBuff != 0);
            var buffApply = noStateActiBuffRem.Where(x => x.IsBuff != 0 && x.BuffDmg == 0 && x.Value > 0);
            var buffInitial = allCombatItems.Where(x => x.IsStateChange == ParseEnum.StateChange.BuffInitial);
            var boonData = new List<CombatItem>(buffRemove);
            boonData.AddRange(buffApply);
            boonData.AddRange(buffInitial);
            boonData.Sort((x, y) => x.Time.CompareTo(y.Time));
            _boonData = boonData.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
            _boonDataByDst = boonData.GroupBy(x => x.IsBuffRemove == ParseEnum.BuffRemove.None ? x.DstInstid : x.SrcInstid).ToDictionary(x => x.Key, x => x.ToList());
            // damage events
            var damageData = noStateActiBuffRem.Where(x => (x.IsBuff != 0 && x.Value == 0) || (x.IsBuff == 0));
            _damageData = damageData.GroupBy(x => x.SrcInstid).ToDictionary(x => x.Key, x => x.ToList());
            _damageTakenData = damageData.GroupBy(x => x.DstInstid).ToDictionary(x => x.Key, x => x.ToList());

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
                return data.Where(x => x.SrcInstid == srcInstid && x.Time >= start && x.Time <= end).ToList();
            }
            return new List<CombatItem>();
        }

        public List<CombatItem> GetBuffs(ushort srcInstid, long skillId, long start, long end)
        {
            if (_boonData.TryGetValue(skillId, out List<CombatItem> data))
            {
                return data.Where(x => x.SrcInstid == srcInstid && x.Time >= start && x.Time <= end && x.IsBuffRemove == ParseEnum.BuffRemove.None).ToList();
            }
            return new List<CombatItem>();
        }

        public void Update(long end)
        {
            List<CombatItem> damageData = _damageData.SelectMany(x => x.Value).ToList();
            damageData.Sort((x, y) => x.Time.CompareTo(y.Time));
            damageData.Reverse();
            bool sortCombatList = false;
            foreach (CombatItem c in damageData)
            {
                if (c.Time <= end)
                {
                    break;
                }
                else if (c.Time <= end + 1000)
                {
                    sortCombatList = true;
                    c.OverrideTime(end);
                }
            }
            if (sortCombatList)
            {
                AllCombatItems.Sort((x, y) => x.Time.CompareTo(y.Time));
            }
        }

        // getters

        public HashSet<long> GetSkills()
        {
            return new HashSet<long>(_skillData.Keys);
        }

        public List<CombatItem> GetSkillData(long key)
        {
            if (_skillData.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }

        public List<CombatItem> GetBoonData(long key)
        {
            if (_boonData.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }

        public List<CombatItem> GetBoonDataByDst(ushort key, long start, long end)
        {
            if (_boonDataByDst.TryGetValue(key, out List<CombatItem> res))
            {
                return res.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return new List<CombatItem>(); ;
        }


        public List<CombatItem> GetDamageData(ushort key, long start, long end)
        {
            if (_damageData.TryGetValue(key, out List<CombatItem> res))
            {
                return res.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return new List<CombatItem>(); ;
        }

        public List<CombatItem> GetCastData(ushort key, long start, long end)
        {
            if (_castData.TryGetValue(key, out List<CombatItem> res))
            {
                return res.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return new List<CombatItem>(); ;
        }


        public List<CombatItem> GetCastDataById(long key)
        {
            if (_castDataById.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }

        public List<CombatItem> GetDamageTakenData(ushort key, long start, long end)
        {
            if (_damageTakenData.TryGetValue(key, out List<CombatItem> res))
            {
                return res.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return new List<CombatItem>();
        }

        /*public List<CombatItem> getHealingData()
        {
            return _healingData;
        }

        public List<CombatItem> getHealingReceivedData()
        {
            return _healingReceivedData;
        }*/


        public List<CombatItem> GetMovementData(ushort key, long start, long end)
        {
            if (_movementData.TryGetValue(key, out List<CombatItem> res))
            {
                return res.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return new List<CombatItem>();
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
            status = status.OrderBy(x => x.Time).ToList();
            for (var i = 0; i < status.Count - 1; i++)
            {
                CombatItem cur = status[i];
                CombatItem next = status[i + 1];
                if (cur.IsStateChange == ParseEnum.StateChange.ChangeDown)
                {
                    down.Add((cur.Time - fightStart, next.Time - fightStart));
                }
                else if (cur.IsStateChange == ParseEnum.StateChange.ChangeDead)
                {
                    dead.Add((cur.Time - fightStart, next.Time - fightStart));
                }
                else if (cur.IsStateChange == ParseEnum.StateChange.Despawn)
                {
                    dc.Add((cur.Time - fightStart, next.Time - fightStart));
                }
            }
            // check last value
            if (status.Count > 0)
            {
                CombatItem cur = status.Last();
                if (cur.IsStateChange == ParseEnum.StateChange.ChangeDown)
                {
                    down.Add((cur.Time - fightStart, fightEnd - fightStart));
                }
                else if (cur.IsStateChange == ParseEnum.StateChange.ChangeDead)
                {
                    dead.Add((cur.Time - fightStart, fightEnd - fightStart));
                }
                else if (cur.IsStateChange == ParseEnum.StateChange.Despawn)
                {
                    dc.Add((cur.Time - fightStart, fightEnd - fightStart));
                }
            }
        }

    }
}