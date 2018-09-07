using LuckParser.Models.ParseModels;
using System.Collections.Generic;

namespace LuckParser.Models.DataModels
{
    public class ParsedLog
    {
        public readonly LogData LogData;
        public readonly FightData FightData;
        public readonly AgentData AgentData;
        public readonly SkillData SkillData;
        public readonly CombatData CombatData;
        public readonly MechanicData MechanicData;
        public readonly List<Player> PlayerList;
        public readonly Boss Boss;

        

        public ParsedLog(LogData logData, FightData fightData, AgentData agentData, SkillData skillData, 
                CombatData combatData, List<Player> playerList, Boss boss)
        {
            LogData = logData;
            FightData = fightData;
            AgentData = agentData;
            SkillData = skillData;
            CombatData = combatData;
            PlayerList = playerList;
            Boss = boss;
            MechanicData = new MechanicData(fightData);
        }

        public Dictionary<long, List<CombatItem>> GetBoonData()
        {
            return CombatData.BoonData;
        }

        public List<CombatItem> GetBoonData(long key)
        {
            return CombatData.GetBoonData(key);
        }

        public Dictionary<ushort, List<CombatItem>> GetBoonDataByDst()
        {
            return CombatData.BoonDataByDst;
        }

        public List<CombatItem> GetBoonDataByDst(ushort key)
        {
            return CombatData.GetBoonDataByDst(key);
        }

        public Dictionary<ushort, List<CombatItem>> GetDamageData()
        {
            return CombatData.DamageData;
        }

        public List<CombatItem> GetDamageData(ushort key)
        {
            return CombatData.GetDamageData(key);
        }

        public Dictionary<ushort, List<CombatItem>> GetCastData()
        {
            return CombatData.CastData;
        }

        public List<CombatItem> GetCastData(ushort key)
        {
            return CombatData.GetCastData(key);
        }

        public Dictionary<long, List<CombatItem>> GetCastDataById()
        {
            return CombatData.CastDataById;
        }
        public List<CombatItem> GetCastDataById(long key)
        {
            return CombatData.GetCastDataById(key);
        }

        public Dictionary<ushort, List<CombatItem>> GetDamageTakenData()
        {
            return CombatData.DamageTakenData;
        }

        public List<CombatItem> GetDamageTakenData(ushort key)
        {
            return CombatData.GetDamageTakenData(key);
        }

        public bool IsBenchmarkMode()
        {
            return FightData.Logic.GetMode() == BossLogic.ParseMode.Golem;
        }


        /*public List<CombatItem> getHealingData()
        {
            return _combatData.getHealingData();
        }

        public List<CombatItem> getHealingReceivedData()
        {
            return _combatData.getHealingReceivedData();
        }*/

        public Dictionary<ushort, List<CombatItem>> GetMovementData()
        {
            return CombatData.MovementData;
        }

        public List<CombatItem> GetMovementData(ushort key)
        {
            return CombatData.GetMovementData(key);
        }
    }
}
