using LuckParser.Models.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class CombatData
    {
        public List<CombatItem> AllCombatItems { get; private set; }

        public Dictionary<ParseEnum.StateChange, List<CombatItem>> StatesData { get; private set; }
        //private List<CombatItem> _healingData;
        //private List<CombatItem> _healingReceivedData;

        public Dictionary<long, List<CombatItem>> BoonData { get; private set; }
        public Dictionary<ushort, List<CombatItem>> BoonDataByDst { get; private set; }
        public Dictionary<ushort, List<CombatItem>> DamageData { get; private set; }
        public Dictionary<ushort, List<CombatItem>> CastData { get; private set; }
        public Dictionary<long, List<CombatItem>> CastDataById { get; private set; }
        public Dictionary<ushort, List<CombatItem>> DamageTakenData { get; private set; }
        public Dictionary<ushort, List<CombatItem>> MovementData { get; private set; }

        public CombatData(List<CombatItem> allCombatItems, FightData fightData)
        {
            AllCombatItems = allCombatItems;
            var noStateActiBuffRem = allCombatItems.Where(x => x.IsStateChange == ParseEnum.StateChange.Normal && x.IsActivation == ParseEnum.Activation.None && x.IsBuffRemove == ParseEnum.BuffRemove.None);
            // movement events
            MovementData = fightData.Logic.CanCombatReplay
                ? allCombatItems.Where(x =>
                        x.IsStateChange == ParseEnum.StateChange.Position ||
                        x.IsStateChange == ParseEnum.StateChange.Velocity ||
                        x.IsStateChange == ParseEnum.StateChange.Rotation).GroupBy(x => x.SrcInstid)
                    .ToDictionary(x => x.Key, x => x.ToList())
                : new Dictionary<ushort, List<CombatItem>>();
            fightData.Logic.CanCombatReplay = MovementData.Count > 1;
            // state change events
            StatesData = allCombatItems.GroupBy(x => x.IsStateChange).ToDictionary(x => x.Key, x => x.ToList());
            // activation events
            var castData = allCombatItems.Where(x => x.IsActivation != ParseEnum.Activation.None || x.IsStateChange == ParseEnum.StateChange.WeaponSwap);
            CastData = castData.GroupBy(x => x.SrcInstid).ToDictionary(x => x.Key, x => x.ToList());
            CastDataById = castData.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
            // buff remove event
            var buffRemove = allCombatItems.Where(x => x.IsBuffRemove != ParseEnum.BuffRemove.None && x.IsBuff != 0);
            var buffApply = noStateActiBuffRem.Where(x => x.IsBuff != 0 && x.BuffDmg == 0);
            var buffInitial = allCombatItems.Where(x => x.IsStateChange == ParseEnum.StateChange.BuffInitial);
            var boonData = new List<CombatItem>(buffRemove);
            boonData.AddRange(buffApply);
            boonData.AddRange(buffInitial);
            boonData.Sort((x, y) => x.Time < y.Time ? -1 : 1);
            BoonData = boonData.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
            BoonDataByDst = boonData.GroupBy(x => x.IsBuffRemove == ParseEnum.BuffRemove.None ? x.DstInstid : x.SrcInstid).ToDictionary(x => x.Key, x => x.ToList());
            // damage events
            var damageData = noStateActiBuffRem.Where(x => (x.IsBuff != 0 && x.Value == 0) || (x.IsBuff == 0));
            DamageData = damageData.GroupBy(x => x.SrcInstid).ToDictionary(x => x.Key, x => x.ToList());
            DamageTakenData = damageData.GroupBy(x => x.DstInstid).ToDictionary(x => x.Key, x => x.ToList());

            /*healing_data = allCombatItems.Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                         ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                         (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            healing_received_data = allCombatItems.Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();*/
        }

        public List<CombatItem> GetStates(int srcInstid, ParseEnum.StateChange change, long start, long end)
        {
            if (StatesData.TryGetValue(change, out List<CombatItem> data))
            {
                return data.Where(x => x.SrcInstid == srcInstid && x.Time >= start && x.Time <= end).ToList();
            }
            return new List<CombatItem>();
        }

        public int GetSkillCount(int srcInstid, long skillId, long start, long end)
        {
            if (CastDataById.TryGetValue(skillId, out List<CombatItem> data))
            {
                return data.Count(x => x.SrcInstid == srcInstid && x.Time >= start && x.Time <= end && x.IsActivation.IsCasting());
            }
            return 0;
        }

        public int GetBuffCount(int srcInstid, long skillId, long start, long end)
        {
            if (BoonData.TryGetValue(skillId, out List<CombatItem> data))
            {
                return data.Count(x => x.SrcInstid == srcInstid && x.Time >= start && x.Time <= end && x.IsBuffRemove == ParseEnum.BuffRemove.None);
            }
            return 0;
        }

        public void Update(long end)
        {
            List<CombatItem> damageData = DamageData.SelectMany(x => x.Value).ToList();
            damageData.Sort((x, y) => x.Time < y.Time ? -1 : 1);
            damageData.Reverse();
            foreach (CombatItem c in damageData)
            {
                if (c.Time <= end)
                {
                    break;
                }
                else if (c.Time <= end + 1000)
                {
                    c.Time = end;
                }
            }
        }

        // getters

        public List<CombatItem> GetBoonData(long key)
        {
            if (BoonData.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }

        public List<CombatItem> GetBoonDataByDst(ushort key)
        {
            if (BoonDataByDst.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }


        public List<CombatItem> GetDamageData(ushort key)
        {
            if (DamageData.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }

        public List<CombatItem> GetCastData(ushort key)
        {
            if (CastData.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }


        public List<CombatItem> GetCastDataById(long key)
        {
            if (CastDataById.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }

        public List<CombatItem> GetDamageTakenData(ushort key)
        {
            if (DamageTakenData.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
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


        public List<CombatItem> GetMovementData(ushort key)
        {
            if (MovementData.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>();
        }

        public List<CombatItem> GetStatesData(ParseEnum.StateChange key)
        {
            if (StatesData.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>();
        }
    }
}