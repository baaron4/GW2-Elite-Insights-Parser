using System.Collections.Generic;
using System.Linq;
using LuckParser.Models.ParseModels;

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
        public readonly Dictionary<string, List<Player>> PlayerListBySpec;
        public readonly Target LegacyTarget;

        

        public ParsedLog(LogData logData, FightData fightData, AgentData agentData, SkillData skillData, 
                CombatData combatData, List<Player> playerList, Target target)
        {
            LogData = logData;
            FightData = fightData;
            AgentData = agentData;
            SkillData = skillData;
            CombatData = combatData;
            PlayerList = playerList;
            LegacyTarget = target;
            MechanicData = new MechanicData(fightData);
            PlayerListBySpec = playerList.GroupBy(x => x.Prof).ToDictionary(x => x.Key, x => x.ToList());

            FightData.SetSuccess(this);
            FightData.SetCM(this);
            CombatData.Update(FightData.FightEnd);
        }

        public List<CombatItem> GetBoonData(long key)
        {
            return CombatData.GetBoonData(key);
        }

        public List<CombatItem> GetBoonDataByDst(ushort key, long start, long end)
        {
            return CombatData.GetBoonDataByDst(key, start, end);
        }

        public List<CombatItem> GetDamageData(ushort key, long start, long end)
        {
            return CombatData.GetDamageData(key, start, end);
        }

        public List<CombatItem> GetCastData(ushort key, long start, long end)
        {
            return CombatData.GetCastData(key, start, end);
        }
        
        public List<CombatItem> GetCastDataById(long key)
        {
            return CombatData.GetCastDataById(key);
        }
        
        public List<CombatItem> GetDamageTakenData(ushort key, long start, long end)
        {
            return CombatData.GetDamageTakenData(key,start,end);
        }

        public bool IsBenchmarkMode()
        {
            return FightData.Logic.Mode == FightLogic.ParseMode.Golem;
        }


        /*public List<CombatItem> getHealingData()
        {
            return _combatData.getHealingData();
        }

        public List<CombatItem> getHealingReceivedData()
        {
            return _combatData.getHealingReceivedData();
        }*/
        
        public List<CombatItem> GetMovementData(ushort key, long start, long end)
        {
            return CombatData.GetMovementData(key,start,end);
        }

        public List<CombatItem> GetStatesData(ParseEnum.StateChange key)
        {
            return CombatData.GetStatesData(key);
        }
    }
}
