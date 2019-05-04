using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.Exceptions;
using LuckParser.Models;
using LuckParser.Models.Logic;
using LuckParser.Models.ParseModels;

namespace LuckParser.Parser
{
    public class ParsedLog
    {
        private readonly ParsedEvtcContainer _container;

        public LogData LogData => _container.LogData;
        public FightData FightData => _container.FightData;
        public AgentData AgentData => _container.AgentData;
        public SkillData SkillData => _container.SkillData;
        public CombatData CombatData => _container.CombatData;
        public List<Player> PlayerList => _container.PlayerList;
        public HashSet<ushort> PlayerIDs => _container.PlayerIDs;
        public Dictionary<string, List<Player>> PlayerListBySpec => _container.PlayerListBySpec;


        public readonly MechanicData MechanicData;
        public readonly BoonSourceFinder BoonSourceFinder;
        public readonly DamageModifiersContainer DamageModifiers;
        public readonly BoonsContainer Boons;
        public bool IsBenchmarkMode => FightData.Logic.Mode == FightLogic.ParseMode.Golem;
        public readonly Target LegacyTarget;
        public readonly Statistics Statistics;

        public ParsedLog(LogData logData, FightData fightData, AgentData agentData, SkillData skillData, 
                CombatData combatData, List<Player> playerList, Target target)
        {
            _container = new ParsedEvtcContainer(logData, fightData, agentData, skillData, combatData, playerList);
            //
            FightData.SetSuccess(_container);
            if (FightData.FightDuration <= 2200)
            {
                throw new TooShortException();
            }
            if (Properties.Settings.Default.SkipFailedTries && !FightData.Success)
            {
                throw new SkipException();
            }
            CombatData.Update(FightData.FightEnd);
            FightData.SetCM(_container);
            //
            Boons = new BoonsContainer(logData.GW2Version);
            BoonSourceFinder = Boon.GetBoonSourceFinder(logData.GW2Version, Boons);
            DamageModifiers = new DamageModifiersContainer(logData.GW2Version);
            MechanicData = new MechanicData(this);
            Statistics = new Statistics(this);
            LegacyTarget = target;
        }
    }
}
