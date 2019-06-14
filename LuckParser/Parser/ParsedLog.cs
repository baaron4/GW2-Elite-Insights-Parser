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
        private readonly List<Mob> _auxMobs = new List<Mob>();

        public LogData LogData { get; }
        public FightData FightData { get; }
        public AgentData AgentData { get; }
        private readonly SkillData _skillData;
        public CombatData CombatData { get; }
        public List<Player> PlayerList { get; }
        public HashSet<AgentItem> PlayerAgents { get; }
        public bool IsBenchmarkMode => FightData.Logic.Mode == FightLogic.ParseMode.Golem;
        public Dictionary<string, List<Player>> PlayerListBySpec { get; }
        public DamageModifiersContainer DamageModifiers { get; }
        public BoonsContainer Boons { get; }
        public bool CanCombatReplay => CombatData.HasMovementData && FightData.Logic.HasCombatReplayMap;

        public readonly MechanicData MechanicData;
        public readonly Target LegacyTarget;
        public readonly Statistics Statistics;

        public ParsedLog(string buildVersion, FightData fightData, AgentData agentData, SkillData skillData, 
                List<CombatItem> combatItems, List<Player> playerList)
        {
            FightData = fightData;
            AgentData = agentData;
            _skillData = skillData;
            PlayerList = playerList;
            //
            PlayerListBySpec = playerList.GroupBy(x => x.Prof).ToDictionary(x => x.Key, x => x.ToList());
            PlayerAgents = new HashSet<AgentItem>(playerList.Select(x => x.AgentItem));
            CombatData = new CombatData(combatItems, FightData, AgentData, _skillData, playerList);
            LogData = new LogData(buildVersion, CombatData, combatItems);
            //
            UpdateFightData();
            //
            Boons = new BoonsContainer(LogData.GW2Version);
            DamageModifiers = new DamageModifiersContainer(LogData.GW2Version);
            MechanicData = FightData.Logic.GetMechanicData();
            Statistics = new Statistics(CombatData, AgentData, FightData, PlayerList, Boons);

            LegacyTarget = FightData.Logic.Targets.Find(x => x.ID == FightData.ID && x.AgentItem.Type == AgentItem.AgentType.NPC);
            if (LegacyTarget == null)
            {
                LegacyTarget = new Target(GeneralHelper.UnknownAgent);
            }
        }

        private void UpdateFightData()
        {
            FightData.Logic.ComputeFightTargets(AgentData, CombatData);
            FightData.Logic.CheckSuccess(CombatData, FightData, PlayerAgents);
            if (FightData.FightDuration <= 2200)
            {
                throw new TooShortException();
            }
            if (Properties.Settings.Default.SkipFailedTries && !FightData.Success)
            {
                throw new SkipException();
            }
            CombatData.UpdateDamageEvents(FightData.FightDuration);
            FightData.SetCM(CombatData, AgentData, FightData);
        }

        /// <summary>
        /// Find the corresponding actor, creates one otherwise
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public AbstractActor FindActor(AgentItem a)
        {
            AbstractActor res = PlayerList.FirstOrDefault(x => x.AgentItem == a);
            if (res == null)
            {
                foreach (Player p in PlayerList)
                {
                    Dictionary<string, Minions> minionsDict = p.GetMinions(this);
                    foreach (Minions minions in minionsDict.Values)
                    {
                        res = minions.FirstOrDefault(x => x.AgentItem == a);
                        if (res != null)
                        {
                            return res;
                        }
                    }
                }
                res = FightData.Logic.Targets.FirstOrDefault(x => x.AgentItem == a);
                if (res == null)
                {
                    res = FightData.Logic.TrashMobs.FirstOrDefault(x => x.AgentItem == a);
                    if (res == null)
                    {
                        res = _auxMobs.FirstOrDefault(x => x.AgentItem == a);
                        if (res == null)
                        {
                            _auxMobs.Add(new Mob(a));
                            res = _auxMobs.Last();
                        }
                    }
                }
            }
            return res;
        }
    }
}
