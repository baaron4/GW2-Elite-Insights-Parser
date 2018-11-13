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

        public Dictionary<ushort, List<CombatItem>> GetMovementData()
        {
            return CombatData.MovementData;
        }

        public List<CombatItem> GetMovementData(ushort key)
        {
            return CombatData.GetMovementData(key);
        }

        public List<CombatItem> GetStatesData(ParseEnum.StateChange key)
        {
            return CombatData.GetStatesData(key);
        }
    }
}
