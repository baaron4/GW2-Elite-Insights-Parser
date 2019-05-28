using LuckParser.Exceptions;
using LuckParser.Models.Logic;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Parser
{
    public class ParsedEvtcContainer
    {
        public readonly LogData LogData;
        public readonly FightData FightData;
        public readonly AgentData AgentData;
        public readonly SkillData SkillData;
        public readonly CombatData CombatData;
        public readonly List<Player> PlayerList;
        public readonly HashSet<ushort> PlayerIDs;
        public bool IsBenchmarkMode => FightData.Logic.Mode == FightLogic.ParseMode.Golem;
        public readonly Dictionary<string, List<Player>> PlayerListBySpec;
        public readonly DamageModifiersContainer DamageModifiers;
        public readonly BoonsContainer Boons;

        public ParsedEvtcContainer(LogData logData, FightData fightData, AgentData agentData, SkillData skillData,
                List<CombatItem> combatItems, List<Player> playerList)
        {
            LogData = logData;
            FightData = fightData;
            AgentData = agentData;
            SkillData = skillData;
            CombatData = new CombatData(combatItems, fightData, agentData, playerList);
            PlayerList = playerList;
            Boons = new BoonsContainer(logData.GW2Version);
            DamageModifiers = new DamageModifiersContainer(logData.GW2Version);
            PlayerListBySpec = playerList.GroupBy(x => x.Prof).ToDictionary(x => x.Key, x => x.ToList());
            PlayerIDs = new HashSet<ushort>(playerList.Select(x => x.InstID));
        }
    }
}
