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
        private MechanicData mech_data;
        private List<Player> p_list = new List<Player>();
        private Boss boss;

        

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
            this.mech_data = new MechanicData(boss_data);
        }

        public BossData GetBossData()
        {
            return boss_data;
        }

        public Boss GetBoss()
        {
            return boss;
        }

        public CombatData GetCombatData()
        {
            return combat_data;
        }

        public AgentData GetAgentData()
        {
            return agent_data;
        }

        public List<Player> GetPlayerList()
        {
            return p_list;
        }

        public MechanicData GetMechanicData()
        {
            return mech_data;
        }

        public SkillData GetSkillData()
        {
            return skill_data;
        }

        public LogData GetLogData()
        {
            return log_data;
        }

        public CombatData GetCombatList()
        {
            return combat_data;
        }

        public List<CombatItem> GetBoonData()
        {
            return combat_data.GetBoonData();
        }

        public List<CombatItem> GetDamageData()
        {
            return combat_data.GetDamageData();
        }

        public List<CombatItem> GetCastData()
        {
            return combat_data.GetCastData();
        }

        public List<CombatItem> GetDamageTakenData()
        {
            return combat_data.GetDamageTakenData();
        }

        public bool IsBenchmarkMode()
        {
            return boss_data.getBossBehavior().getMode() == BossLogic.ParseMode.Golem;
        }

        /*public List<CombatItem> getHealingData()
        {
            return combat_data.getHealingData();
        }

        public List<CombatItem> getHealingReceivedData()
        {
            return combat_data.getHealingReceivedData();
        }*/

        public List<CombatItem> GetMovementData()
        {
            return combat_data.GetMovementData();
        }     
    }
}
