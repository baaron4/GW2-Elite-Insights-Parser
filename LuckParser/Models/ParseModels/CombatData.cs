using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class CombatData : List<CombatItem>
    {
        // reduced data
        private List<CombatItem> boon_data;
        private List<CombatItem> damage_data;
        private List<CombatItem> damage_taken_data;
        //private List<CombatItem> healing_data;
        //private List<CombatItem> healing_received_data;
        private List<CombatItem> cast_data;
        private List<CombatItem> movement_data;

        // Constructors
        public CombatData()
        {
        }

        // Public Methods

        public List<CombatItem> GetStates(int src_instid, ParseEnum.StateChange change, long start, long end)
        {
            List<CombatItem> states = new List<CombatItem>();
            foreach (CombatItem c in this.Where(x => x.GetTime() >= start && x.GetTime() <= end))
            {
                if (c.GetSrcInstid() == src_instid && c.IsStateChange() == change)
                {
                    states.Add(c);
                }
            }
            return states;
        }

        public int GetSkillCount(int src_instid, int skill_id, long start, long end)
        {
            int count = 0;
            foreach (CombatItem c in this.Where(x => x.GetTime() >= start && x.GetTime() <= end))
            {
                if (c.GetSrcInstid() == src_instid && c.GetSkillID() == skill_id)
                {
                    if (c.IsActivation().IsCasting())
                        count++;
                }
            }
            return count;
        }
        public int GetBuffCount(int src_instid, int skill_id, long start, long end)
        {
            int count = 0;
            foreach (CombatItem c in this.Where(x => x.GetTime() >= start && x.GetTime() <= end))
            {
                if (c.GetSrcInstid() == src_instid && c.GetSkillID() == skill_id)
                {
                    if (c.IsBuff() == 1 && c.IsBuffremove() == ParseEnum.BuffRemove.None)
                        count++;
                }
            }
            return count;
        }
        public void Validate(BossData boss_data)
        {
            boon_data = this.Where(x => x.IsBuff() > 0 && (x.IsBuff() == 18 || x.GetBuffDmg() == 0 || x.IsBuffremove() != ParseEnum.BuffRemove.None)).ToList();

            damage_data = this.Where(x => x.GetDstInstid() != 0 && x.IsStateChange() == ParseEnum.StateChange.Normal && x.GetIFF() == ParseEnum.IFF.Foe && x.IsBuffremove() == ParseEnum.BuffRemove.None &&
                                        ((x.IsBuff() == 1 && x.GetBuffDmg() > 0 && x.GetValue() == 0) ||
                                        (x.IsBuff() == 0 && x.GetValue() > 0))).ToList();

            damage_taken_data = this.Where(x => x.IsStateChange() == ParseEnum.StateChange.Normal && x.IsBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.IsBuff() == 1 && x.GetBuffDmg() >= 0 && x.GetValue() == 0) ||
                                                (x.IsBuff() == 0 && x.GetValue() >= 0))).ToList();

            cast_data = this.Where(x => (x.IsStateChange() == ParseEnum.StateChange.Normal && x.IsActivation() != ParseEnum.Activation.None) || x.IsStateChange() == ParseEnum.StateChange.WeaponSwap).ToList();

            movement_data = (boss_data.GetBossBehavior().GetMode() == BossLogic.ParseMode.Fractal || boss_data.GetBossBehavior().GetMode() == BossLogic.ParseMode.Raid) ? this.Where(x => x.IsStateChange() == ParseEnum.StateChange.Position || x.IsStateChange() == ParseEnum.StateChange.Velocity).ToList() : new List<CombatItem>();

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
            return boon_data;
        }

        public List<CombatItem> GetDamageData()
        {
            return damage_data;
        }

        public List<CombatItem> GetCastData()
        {
            return cast_data;
        }

        public List<CombatItem> GetDamageTakenData()
        {
            return damage_taken_data;
        }

        /*public List<CombatItem> getHealingData()
        {
            return healing_data;
        }

        public List<CombatItem> getHealingReceivedData()
        {
            return healing_received_data;
        }*/

        public List<CombatItem> GetMovementData()
        {
            return movement_data;
        }
    }
}