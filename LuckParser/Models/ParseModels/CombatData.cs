using LuckParser.Models.DataModels;
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
        // Public Methods

        public List<CombatItem> GetStates(int srcInstid, ParseEnum.StateChange change, long start, long end)
        {
            var list = new List<CombatItem>();
            foreach (var combatItem in this)
            {
                if (combatItem.Time >= start && combatItem.Time <= end &&
                    combatItem.SrcInstid == srcInstid && combatItem.IsStateChange == change)
                {
                    list.Add(combatItem);
                }
            }

            return list;
        }

        public int GetSkillCount(int srcInstid, int skillId, long start, long end)
        {
            int count = 0;
            foreach (CombatItem c in this.Where(x => x.Time >= start && x.Time <= end))
            {
                if (c.SrcInstid == srcInstid && c.SkillID == skillId)
                {
                    if (c.IsActivation.IsCasting())
                        count++;
                }
            }
            return count;
        }
        public int GetBuffCount(int srcInstid, int skillId, long start, long end)
        {
            int count = 0;
            foreach (CombatItem c in this.Where(x => x.Time >= start && x.Time <= end))
            {
                if (c.SrcInstid == srcInstid && c.SkillID == skillId)
                {
                    if (c.IsBuff == 1 && c.IsBuffRemove == ParseEnum.BuffRemove.None)
                        count++;
                }
            }
            return count;
        }
        public void Validate(BossData bossData)
        {
            _boonData = this.Where(x => x.IsBuff > 0 && (x.IsBuff == 18 || x.BuffDmg == 0 || x.IsBuffRemove != ParseEnum.BuffRemove.None)).ToList();

            _damageData = this.Where(x => x.DstInstid != 0 && x.IsStateChange == ParseEnum.StateChange.Normal && x.IFF == ParseEnum.IFF.Foe && x.IsBuffRemove == ParseEnum.BuffRemove.None &&
                                        ((x.IsBuff == 1 && x.BuffDmg > 0 && x.Value == 0) ||
                                        (x.IsBuff == 0 && x.Value > 0))).ToList();

            _damageTakenData = this.Where(x => x.IsStateChange == ParseEnum.StateChange.Normal && x.IsBuffRemove == ParseEnum.BuffRemove.None &&
                                            ((x.IsBuff == 1 && x.BuffDmg >= 0 && x.Value == 0) ||
                                                (x.IsBuff == 0 && x.Value >= 0))).ToList();

            _castData = this.Where(x => (x.IsStateChange == ParseEnum.StateChange.Normal && x.IsActivation != ParseEnum.Activation.None) || x.IsStateChange == ParseEnum.StateChange.WeaponSwap).ToList();

            _movementData = (bossData.GetBossBehavior().GetMode() == BossLogic.ParseMode.Fractal || bossData.GetBossBehavior().GetMode() == BossLogic.ParseMode.Raid) ? this.Where(x => x.IsStateChange == ParseEnum.StateChange.Position || x.IsStateChange == ParseEnum.StateChange.Velocity).ToList() : new List<CombatItem>();

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