using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Exceptions;
using GW2EIParser.Logic;

namespace GW2EIParser.Parser.ParsedData
{
    public class ParsedLog
    {
        public LogData LogData { get; }
        public FightData FightData { get; }
        public AgentData AgentData { get; }
        public SkillData SkillData { get; }
        public CombatData CombatData { get; }
        public List<Player> PlayerList { get; }
        public HashSet<AgentItem> PlayerAgents { get; }
        public bool IsBenchmarkMode => FightData.Logic.Mode == FightLogic.ParseMode.Golem;
        public Dictionary<string, List<Player>> PlayerListBySpec { get; }
        public DamageModifiersContainer DamageModifiers { get; }
        public BuffsContainer Buffs { get; }
        public ParserSettings ParserSettings { get; }
        public bool CanCombatReplay => ParserSettings.ParseCombatReplay && CombatData.HasMovementData;

        public MechanicData MechanicData { get; }
        public GeneralStatistics Statistics { get; }

        private readonly OperationController _operation;


        private Dictionary<AgentItem, AbstractSingleActor> _agentToActorDictionary;

        public ParsedLog(string buildVersion, FightData fightData, AgentData agentData, SkillData skillData,
                List<CombatItem> combatItems, List<Player> playerList, long evtcLogDuration, ParserSettings parserSettings, OperationController operation)
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
            CombatData = new CombatData(combatItems, FightData, AgentData, SkillData, playerList);
            LogData = new LogData(buildVersion, CombatData, evtcLogDuration);
            //
            UpdateFightData();
            //
            Buffs = new BuffsContainer(LogData.GW2Version);
            DamageModifiers = new DamageModifiersContainer(LogData.GW2Version);
            MechanicData = FightData.Logic.GetMechanicData();
            Statistics = new GeneralStatistics(CombatData, PlayerList, Buffs);
        }

        public void UpdateProgress(string status)
        {
            _operation.UpdateProgress(status);
        }

        public void ThrowIfCanceled()
        {
            _operation.ThrowIfCanceled();
        }

        private void UpdateFightData()
        {
            FightData.Logic.CheckSuccess(CombatData, AgentData, FightData, PlayerAgents);
            if (FightData.FightEnd <= 2200)
            {
                throw new TooShortException();
            }
            if (ParserSettings.SkipFailedTries && !FightData.Success)
            {
                throw new SkipException();
            }
            FightData.SetCM(CombatData, AgentData, FightData);
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
