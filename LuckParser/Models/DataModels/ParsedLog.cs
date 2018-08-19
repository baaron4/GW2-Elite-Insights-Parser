using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckParser.Models.ParseModels;

namespace LuckParser.Models.DataModels
{
    public class ParsedLog
    {
        private LogData log_data;
        private BossData boss_data;
        private AgentData agent_data = new AgentData();
        private SkillData skill_data = new SkillData();
        private CombatData combat_data = new CombatData();
        private List<MechanicLog> mech_data = new List<MechanicLog>();
        private List<Player> p_list = new List<Player>();
        private Boss boss;

        // reduced data
        private List<CombatItem> boon_data;
        private List<CombatItem> damage_data;
        private List<CombatItem> damage_taken_data;
        //private List<CombatItem> healing_data;
        //private List<CombatItem> healing_received_data;
        private List<CombatItem> cast_data;
        private List<CombatItem> movement_data;

        public ParsedLog(LogData log_data, BossData boss_data, AgentData agent_data, SkillData skill_data, 
                CombatData combat_data, List<Player> p_list, Boss boss)
        {
            this.log_data = log_data;
            this.boss_data = boss_data;
            this.agent_data = agent_data;
            this.skill_data = skill_data;
            this.combat_data = combat_data;
            this.p_list = p_list;
            this.boss = boss;
        }

        public BossData getBossData()
        {
            return boss_data;
        }

        public Boss getBoss()
        {
            return boss;
        }

        public CombatData getCombatData()
        {
            return combat_data;
        }

        public AgentData getAgentData()
        {
            return agent_data;
        }

        public List<Player> getPlayerList()
        {
            return p_list;
        }

        public List<MechanicLog> getMechanicData()
        {
            return mech_data;
        }

        public SkillData getSkillData()
        {
            return skill_data;
        }

        public LogData getLogData()
        {
            return log_data;
        }

        public List<CombatItem> getCombatList()
        {
            return combat_data.getCombatList();
        }

        public void validateLogData()
        {
            boon_data = combat_data.getCombatList().Where(x => x.isBuff() > 0 && (x.isBuff() == 18 || x.getBuffDmg() == 0 || x.isBuffremove() != ParseEnum.BuffRemove.None)).ToList();

            damage_data = combat_data.getCombatList().Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Foe && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                        ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                        (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            damage_taken_data = combat_data.getCombatList().Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() >= 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();

            cast_data = combat_data.getCombatList().Where(x => (x.isStateChange() == ParseEnum.StateChange.Normal && x.isActivation() != ParseEnum.Activation.None) || x.isStateChange() == ParseEnum.StateChange.WeaponSwap).ToList();

            movement_data = (boss_data.getBossBehavior().getMode() == BossLogic.ParseMode.Fractal || boss_data.getBossBehavior().getMode() == BossLogic.ParseMode.Raid) ? combat_data.getCombatList().Where(x => x.isStateChange() == ParseEnum.StateChange.Position || x.isStateChange() == ParseEnum.StateChange.Velocity).ToList() : new List<CombatItem>();

            /*healing_data = combat_data.getCombatList().Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                         ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                         (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            healing_received_data = combat_data.getCombatList().Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();*/
        }

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

        public bool isBenchmarkMode()
        {
            return boss_data.getBossBehavior().getMode() == BossLogic.ParseMode.Golem;
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
