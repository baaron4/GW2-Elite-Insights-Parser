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

        public List<CombatItem> getStates(int src_instid, ParseEnum.StateChange change, long start, long end)
        {
            List<CombatItem> states = new List<CombatItem>();
            foreach (CombatItem c in this.Where(x => x.getTime() >= start && x.getTime() <= end))
            {
                if (c.getSrcInstid() == src_instid && c.isStateChange() == change)
                {
                    states.Add(c);
                }
            }
            return states;
        }

        public int getSkillCount(int src_instid, int skill_id, long start, long end)
        {
            int count = 0;
            foreach (CombatItem c in this.Where(x => x.getTime() >= start && x.getTime() <= end))
            {
                if (c.getSrcInstid() == src_instid && c.getSkillID() == skill_id)
                {
                    if (c.isActivation().IsCasting())
                        count++;
                }
            }
            return count;
        }
        public int getBuffCount(int src_instid, int skill_id, long start, long end)
        {
            int count = 0;
            foreach (CombatItem c in this.Where(x => x.getTime() >= start && x.getTime() <= end))
            {
                if (c.getSrcInstid() == src_instid && c.getSkillID() == skill_id)
                {
                    if (c.isBuff() == 1 && c.isBuffremove() == ParseEnum.BuffRemove.None)
                        count++;
                }
            }
            return count;
        }
        public void validate(BossData boss_data)
        {
            boon_data = this.Where(x => x.isBuff() > 0 && (x.isBuff() == 18 || x.getBuffDmg() == 0 || x.isBuffremove() != ParseEnum.BuffRemove.None)).ToList();

            damage_data = this.Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Foe && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                        ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                        (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            damage_taken_data = this.Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() >= 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();

            cast_data = this.Where(x => (x.isStateChange() == ParseEnum.StateChange.Normal && x.isActivation() != ParseEnum.Activation.None) || x.isStateChange() == ParseEnum.StateChange.WeaponSwap).ToList();

            movement_data = (boss_data.getBossBehavior().getMode() == BossLogic.ParseMode.Fractal || boss_data.getBossBehavior().getMode() == BossLogic.ParseMode.Raid) ? this.Where(x => x.isStateChange() == ParseEnum.StateChange.Position || x.isStateChange() == ParseEnum.StateChange.Velocity).ToList() : new List<CombatItem>();

            /*healing_data = this.Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                         ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                         (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            healing_received_data = this.Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();*/
        }
        // getters
        public List<CombatItem> getBoonData()
        {
            return boon_data;
        }

        public List<CombatItem> getDamageData()
        {
            return damage_data;
        }

        public List<CombatItem> getCastData()
        {
            return cast_data;
        }

        public List<CombatItem> getDamageTakenData()
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

        public List<CombatItem> getMovementData()
        {
            return movement_data;
        }
    }
}