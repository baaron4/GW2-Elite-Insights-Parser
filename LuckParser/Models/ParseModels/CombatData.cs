using LuckParser.Models.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class CombatData : List<CombatItem>
    {
        // reduced data
        private Dictionary<long, List<CombatItem>> _boonData;
        private Dictionary<ushort, List<CombatItem>> _boonDataByDst;
        private Dictionary<ushort, List<CombatItem>> _damageData;
        private Dictionary<ushort, List<CombatItem>> _damageTakenData;
        //private List<CombatItem> _healingData;
        //private List<CombatItem> _healingReceivedData;
        private Dictionary<ushort, List<CombatItem>> _castData;
        private Dictionary<long, List<CombatItem>> _castDataById;
        private Dictionary<ushort, List<CombatItem>> _movementData;
        private Dictionary<ParseEnum.StateChange, List<CombatItem>> _statesData;
        // Public Methods

        public List<CombatItem> GetStates(int srcInstid, ParseEnum.StateChange change, long start, long end)
        {
            if (_statesData.TryGetValue(change, out List<CombatItem> data))
            {
                return data.Where(x => x.SrcInstid == srcInstid && x.Time >= start && x.Time <= end).ToList();
            }
            return new List<CombatItem>();
        }

        public int GetSkillCount(int srcInstid, int skillId, long start, long end)
        {
            if (_castDataById.TryGetValue(skillId, out List<CombatItem> data))
            {
                return data.Count(x => x.SrcInstid == srcInstid && x.Time >= start && x.Time <= end && x.IsActivation.IsCasting());
            }
            return 0;
        }
        public int GetBuffCount(int srcInstid, int skillId, long start, long end)
        {
            if (_boonData.TryGetValue(skillId, out List<CombatItem> data))
            {
                return data.Count(x => x.SrcInstid == srcInstid && x.Time >= start && x.Time <= end && x.IsBuffRemove == ParseEnum.BuffRemove.None);
            }
            return 0;
        }
        public void Validate(FightData fightData)
        {
            var boonData = this.Where(x => x.IsBuff > 0 && (x.IsBuff == 18 || x.BuffDmg == 0 || x.IsBuffRemove != ParseEnum.BuffRemove.None));
            _boonData = boonData.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
            _boonDataByDst = boonData.GroupBy(x => x.IsBuffRemove == ParseEnum.BuffRemove.None ? x.DstInstid : x.SrcInstid).ToDictionary(x => x.Key, x => x.ToList());

            var damageData = this.Where(x => x.IsStateChange == ParseEnum.StateChange.Normal && x.IsBuffRemove == ParseEnum.BuffRemove.None &&
                                        ((x.IsBuff == 1 && x.BuffDmg >= 0 && x.Value == 0) ||
                                        (x.IsBuff == 0 && x.Value >= 0)));
            _damageData = damageData.Where(x => x.DstInstid != 0 && x.IFF == ParseEnum.IFF.Foe).GroupBy(x => x.SrcInstid).ToDictionary(x => x.Key, x => x.ToList());
            _damageTakenData = damageData.GroupBy(x => x.DstInstid).ToDictionary(x => x.Key, x => x.ToList());

            var castData = this.Where(x => (x.IsStateChange == ParseEnum.StateChange.Normal && x.IsActivation != ParseEnum.Activation.None) || x.IsStateChange == ParseEnum.StateChange.WeaponSwap);
            _castData = castData.GroupBy(x => x.SrcInstid).ToDictionary(x => x.Key, x => x.ToList());
            _castDataById = castData.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());

            _statesData = this.GroupBy(x => x.IsStateChange).ToDictionary(x => x.Key, x => x.ToList());

            _movementData = (fightData.Logic.CanCombatReplay) ? this.Where(x => x.IsStateChange == ParseEnum.StateChange.Position || x.IsStateChange == ParseEnum.StateChange.Velocity).GroupBy(x => x.SrcInstid).ToDictionary(x => x.Key, x => x.ToList()) : new Dictionary<ushort, List<CombatItem>>();

            /*healing_data = this.Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                         ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                         (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            healing_received_data = this.Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();*/
        }
        // getters
        public Dictionary<long, List<CombatItem>> GetBoonData()
        {
            return _boonData;
        }
        public List<CombatItem> GetBoonData(long key)
        {
            if (_boonData.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }

        public Dictionary<ushort, List<CombatItem>> GetBoonDataByDst()
        {
            return _boonDataByDst;
        }
        public List<CombatItem> GetBoonDataByDst(ushort key)
        {
            if (_boonDataByDst.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }

        public Dictionary<ushort, List<CombatItem>> GetDamageData()
        {
            return _damageData;
        }
        public List<CombatItem> GetDamageData(ushort key)
        {
            if (_damageData.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }

        public Dictionary<ushort, List<CombatItem>> GetCastData()
        {
            return _castData;
        }
        public List<CombatItem> GetCastData(ushort key)
        {
            if (_castData.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }

        public Dictionary<long, List<CombatItem>> GetCastDataById()
        {
            return _castDataById;
        }
        public List<CombatItem> GetCastDataById(long key)
        {
            if (_castDataById.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }

        public Dictionary<ushort, List<CombatItem>> GetDamageTakenData()
        {
            return _damageTakenData;
        }

        public List<CombatItem> GetDamageTakenData(ushort key)
        {
            if (_damageTakenData.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }

        /*public List<CombatItem> getHealingData()
        {
            return _healingData;
        }

        public List<CombatItem> getHealingReceivedData()
        {
            return _healingReceivedData;
        }*/

        public Dictionary<ushort, List<CombatItem>> GetMovementData()
        {
            return _movementData;
        }
        public List<CombatItem> GetMovementData(ushort key)
        {
            if (_movementData.TryGetValue(key, out List<CombatItem> res))
            {
                return res;
            }
            return new List<CombatItem>(); ;
        }
    }
}