using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.Exceptions;

namespace GW2EIEvtcParser
{
    public class ParsedEvtcLog
    {
        public LogData LogData { get; }
        public FightData FightData { get; }
        public AgentData AgentData { get; }
        public SkillData SkillData { get; }
        public CombatData CombatData { get; }
        public List<Player> PlayerList { get; }
        public HashSet<AgentItem> PlayerAgents { get; }
        public bool IsBenchmarkMode => FightData.Logic.Mode == FightLogic.ParseMode.Benchmark;
        public Dictionary<string, List<Player>> PlayerListBySpec { get; }
        public DamageModifiersContainer DamageModifiers { get; }
        public BuffsContainer Buffs { get; }
        public EvtcParserSettings ParserSettings { get; }
        public bool CanCombatReplay => ParserSettings.ParseCombatReplay && CombatData.HasMovementData;

        public MechanicData MechanicData { get; }
        public GeneralStatistics Statistics { get; }

        private readonly ParserController _operation;

        private Dictionary<AgentItem, AbstractSingleActor> _agentToActorDictionary;

        internal ParsedEvtcLog(string buildVersion, FightData fightData, AgentData agentData, SkillData skillData,
                List<CombatItem> combatItems, List<Player> playerList, long evtcLogDuration, EvtcParserSettings parserSettings, ParserController operation)
        {
            FightData = fightData;
            AgentData = agentData;
            SkillData = skillData;
            PlayerList = playerList;
            ParserSettings = parserSettings;
            _operation = operation;
            //
            PlayerListBySpec = playerList.GroupBy(x => x.Prof).ToDictionary(x => x.Key, x => x.ToList());
            PlayerAgents = new HashSet<AgentItem>(playerList.Select(x => x.AgentItem));
            _operation.UpdateProgressWithCancellationCheck("Creating GW2EI Combat Events");
            CombatData = new CombatData(combatItems, FightData, AgentData, SkillData, playerList, operation);
            _operation.UpdateProgressWithCancellationCheck("Creating GW2EI Log Meta Data");
            LogData = new LogData(buildVersion, CombatData, evtcLogDuration, playerList, operation);
            //
            _operation.UpdateProgressWithCancellationCheck("Checking Success");
            FightData.Logic.CheckSuccess(CombatData, AgentData, FightData, PlayerAgents);
            if (FightData.FightEnd <= ParserSettings.TooShortLimit)
            {
                throw new TooShortException(FightData.FightEnd, ParserSettings.TooShortLimit);
            }
            if (ParserSettings.SkipFailedTries && !FightData.Success)
            {
                throw new SkipException();
            }
            _operation.UpdateProgressWithCancellationCheck("Checking CM");
            FightData.SetCM(CombatData, AgentData, FightData);
            //
            _operation.UpdateProgressWithCancellationCheck("Creating Buff Container");
            Buffs = new BuffsContainer(LogData.GW2Build, CombatData, operation);
            _operation.UpdateProgressWithCancellationCheck("Creating Damage Modifier Container");
            DamageModifiers = new DamageModifiersContainer(LogData.GW2Build, fightData.Logic.Mode, parserSettings);
            _operation.UpdateProgressWithCancellationCheck("Creating Mechanic Data");
            MechanicData = FightData.Logic.GetMechanicData();
            _operation.UpdateProgressWithCancellationCheck("Creating General Statistics Container");
            Statistics = new GeneralStatistics(CombatData, PlayerList, Buffs);
        }

        public void UpdateProgressWithCancellationCheck(string status)
        {
            _operation.UpdateProgressWithCancellationCheck(status);
        }

        private void AddToDictionary(AbstractSingleActor actor)
        {
            _agentToActorDictionary[actor.AgentItem] = actor;
            foreach (Minions minions in actor.GetMinions(this).Values)
            {
                foreach (NPC npc in minions.MinionList)
                {
                    AddToDictionary(npc);
                }
            }
        }

        private void InitActorDictionaries()
        {
            if (_agentToActorDictionary == null)
            {
                _operation.UpdateProgressWithCancellationCheck("Initializing Actor dictionary");
                _agentToActorDictionary = new Dictionary<AgentItem, AbstractSingleActor>();
                foreach (Player p in PlayerList)
                {
                    AddToDictionary(p);
                }
                foreach (NPC npc in FightData.Logic.Targets)
                {
                    AddToDictionary(npc);
                }

                foreach (NPC npc in FightData.Logic.TrashMobs)
                {
                    AddToDictionary(npc);
                }
            }
        }

        /// <summary>
        /// Find the corresponding actor, creates one otherwise
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public AbstractSingleActor FindActor(AgentItem a, bool searchPlayers)
        {
            if (a == null || (!searchPlayers && a.Type == AgentItem.AgentType.Player))
            {
                return null;
            }
            InitActorDictionaries();
            if (!_agentToActorDictionary.TryGetValue(a, out AbstractSingleActor actor))
            {
                actor = new NPC(a);
                _agentToActorDictionary[a] = actor;
                //throw new InvalidOperationException("Requested actor with id " + a.ID + " and name " + a.Name + " is missing");
            }
            if (a.Master != null && !searchPlayers && a.Master.Type == AgentItem.AgentType.Player)
            {
                return null;
            }
            return actor;
        }
    }
}
