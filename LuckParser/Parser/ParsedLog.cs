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
        private readonly List<Mob> _auxMobs = new List<Mob>();

        public LogData LogData => _container.LogData;
        public FightData FightData => _container.FightData;
        public AgentData AgentData => _container.AgentData;
        public SkillData SkillData => _container.SkillData;
        public CombatData CombatData => _container.CombatData;
        public List<Player> PlayerList => _container.PlayerList;
        public HashSet<ushort> PlayerIDs => _container.PlayerIDs;
        public HashSet<AgentItem> PlayerAgents => _container.PlayerAgents;
        public Dictionary<string, List<Player>> PlayerListBySpec => _container.PlayerListBySpec;
        public DamageModifiersContainer DamageModifiers => _container.DamageModifiers;
        public BoonsContainer Boons => _container.Boons;
        public bool CanCombatReplay => CombatData.HasMovementData && FightData.Logic.HasCombatReplayMap;


        public readonly MechanicData MechanicData;
        public bool IsBenchmarkMode => FightData.Logic.Mode == FightLogic.ParseMode.Golem;
        public readonly Target LegacyTarget;
        public readonly Statistics Statistics;

        public ParsedLog(LogData logData, FightData fightData, AgentData agentData, SkillData skillData, 
                List<CombatItem> combatItems, List<Player> playerList, Target target)
        {
            _container = new ParsedEvtcContainer(logData, fightData, agentData, skillData, combatItems, playerList);
            //
            FightData.CheckSuccess(_container);
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
            MechanicData = FightData.Logic.GetMechanicData();
            Statistics = new Statistics(_container);
            LegacyTarget = target;
        }

        public AbstractActor FindActor(long time, ushort instid)
        {
            AbstractActor res = PlayerList.FirstOrDefault(x => x.InstID == instid);
            if (res == null)
            {
                foreach (Player p in PlayerList)
                {
                    Dictionary<string, Minions> minionsDict = p.GetMinions(this);
                    foreach (Minions minions in minionsDict.Values)
                    {
                        res = minions.FirstOrDefault(x => x.InstID == instid && x.FirstAware <= time && x.LastAware >= time);
                        if (res != null)
                        {
                            return res;
                        }
                    }
                }
                res = FightData.Logic.Targets.FirstOrDefault(x => x.InstID == instid && x.FirstAware <= time && x.LastAware >= time);
                if (res == null)
                {
                    res = _auxMobs.FirstOrDefault(x => x.InstID == instid && x.FirstAware <= time && x.LastAware >= time);
                    if (res == null)
                    {
                        _auxMobs.Add(new Mob(AgentData.GetAgentByInstID(instid, time)));
                        res = _auxMobs.Last();
                    }
                }
            }
            return res;
        }
    }
}
