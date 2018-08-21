using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class CombatData : List<CombatItem>
    {
        // reduced data
        private List<CombatItem> _boonData;
        private List<CombatItem> _damageData;
        private List<CombatItem> _damageTakenData;
        //private List<CombatItem> _healingData;
        //private List<CombatItem> _healingReceivedData;
        private List<CombatItem> _castData;
        private List<CombatItem> _movementData;

        // Constructors
        public CombatData()
        {
        }

        // Public Methods

        public List<CombatItem> GetStates(int srcInstid, ParseEnum.StateChange change, long start, long end)
        {
            List<CombatItem> states = new List<CombatItem>();
            foreach (CombatItem c in this.Where(x => x.GetTime() >= start && x.GetTime() <= end))
            {
                if (c.GetSrcInstid() == srcInstid && c.IsStateChange() == change)
                {
                    states.Add(c);
                }
            }
            return states;
        }

        public int GetSkillCount(int srcInstid, int skillId, long start, long end)
        {
            int count = 0;
            foreach (CombatItem c in this.Where(x => x.GetTime() >= start && x.GetTime() <= end))
            {
                if (c.GetSrcInstid() == srcInstid && c.GetSkillID() == skillId)
                {
                    if (c.IsActivation().IsCasting())
                        count++;
                }
            }
            return count;
        }
        public int GetBuffCount(int srcInstid, int skillId, long start, long end)
        {
            int count = 0;
            foreach (CombatItem c in this.Where(x => x.GetTime() >= start && x.GetTime() <= end))
            {
                if (c.GetSrcInstid() == srcInstid && c.GetSkillID() == skillId)
                {
                    if (c.IsBuff() == 1 && c.IsBuffremove() == ParseEnum.BuffRemove.None)
                        count++;
                }
            }
            return count;
        }
        public void Validate(BossData bossData)
        {
            _boonData = this.Where(x => x.IsBuff() > 0 && (x.IsBuff() == 18 || x.GetBuffDmg() == 0 || x.IsBuffremove() != ParseEnum.BuffRemove.None)).ToList();

            _damageData = this.Where(x => x.GetDstInstid() != 0 && x.IsStateChange() == ParseEnum.StateChange.Normal && x.GetIFF() == ParseEnum.IFF.Foe && x.IsBuffremove() == ParseEnum.BuffRemove.None &&
                                        ((x.IsBuff() == 1 && x.GetBuffDmg() > 0 && x.GetValue() == 0) ||
                                        (x.IsBuff() == 0 && x.GetValue() > 0))).ToList();

            _damageTakenData = this.Where(x => x.IsStateChange() == ParseEnum.StateChange.Normal && x.IsBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.IsBuff() == 1 && x.GetBuffDmg() >= 0 && x.GetValue() == 0) ||
                                                (x.IsBuff() == 0 && x.GetValue() >= 0))).ToList();

            _castData = this.Where(x => (x.IsStateChange() == ParseEnum.StateChange.Normal && x.IsActivation() != ParseEnum.Activation.None) || x.IsStateChange() == ParseEnum.StateChange.WeaponSwap).ToList();

            _movementData = (bossData.GetBossBehavior().GetMode() == BossLogic.ParseMode.Fractal || bossData.GetBossBehavior().GetMode() == BossLogic.ParseMode.Raid) ? this.Where(x => x.IsStateChange() == ParseEnum.StateChange.Position || x.IsStateChange() == ParseEnum.StateChange.Velocity).ToList() : new List<CombatItem>();

            /*healing_data = this.Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                         ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                         (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            healing_received_data = this.Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();*/
        }
        // getters
        public List<CombatItem> GetBoonData()
        {
            return _boonData;
        }

        public List<CombatItem> GetDamageData()
        {
            return _damageData;
        }

        public List<CombatItem> GetCastData()
        {
            return _castData;
        }

        public List<CombatItem> GetDamageTakenData()
        {
            return _damageTakenData;
        }

        /*public List<CombatItem> getHealingData()
        {
            return _healingData;
        }

        public List<CombatItem> getHealingReceivedData()
        {
            return _healingReceivedData;
        }*/

        public List<CombatItem> GetMovementData()
        {
            return _movementData;
        }
    }
}