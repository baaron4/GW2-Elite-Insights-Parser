using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser
{
    public class ParsedEvtcLog
    {
        public LogData LogData { get; }
        public FightData FightData { get; }
        public AgentData AgentData { get; }
        public SkillData SkillData { get; }
        public CombatData CombatData { get; }
        public IReadOnlyList<Player> PlayerList { get; }
        public IReadOnlyList<AbstractSingleActor> Friendlies { get; }
        public IReadOnlyCollection<AgentItem> PlayerAgents { get; }
        public IReadOnlyCollection<AgentItem> FriendlyAgents { get; }
        public bool IsBenchmarkMode => FightData.Logic.Mode == FightLogic.ParseMode.Benchmark;
        public IReadOnlyDictionary<ParserHelper.Spec, List<AbstractSingleActor>> FriendliesListBySpec { get; }
        public DamageModifiersContainer DamageModifiers { get; }
        public BuffsContainer Buffs { get; }
        public EvtcParserSettings ParserSettings { get; }
        public bool CanCombatReplay => ParserSettings.ParseCombatReplay && CombatData.HasMovementData;

        public MechanicData MechanicData { get; }
        public StatisticsHelper StatisticsHelper { get; }

        private readonly ParserController _operation;

        private Dictionary<AgentItem, AbstractSingleActor> _agentToActorDictionary;

        internal ParsedEvtcLog(int evtcVersion, FightData fightData, AgentData agentData, SkillData skillData,
                List<CombatItem> combatItems, List<Player> playerList, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions, EvtcParserSettings parserSettings, ParserController operation)
        {
            FightData = fightData;
            AgentData = agentData;
            SkillData = skillData;
            PlayerList = playerList;
            PlayerAgents = new HashSet<AgentItem>(PlayerList.Select(x => x.AgentItem));
            ParserSettings = parserSettings;
            _operation = operation;
            if (parserSettings.AnonymousPlayers)
            {
                operation.UpdateProgressWithCancellationCheck("Anonymous players");
                for (int i = 0; i < PlayerList.Count; i++)
                {
                    PlayerList[i].Anonymize(i + 1);
                }
                IReadOnlyList<AgentItem> allPlayerAgents = agentData.GetAgentByType(AgentItem.AgentType.Player);
                int playerOffset = PlayerList.Count;
                foreach (AgentItem playerAgent in allPlayerAgents)
                {
                    if (!PlayerAgents.Contains(playerAgent))
                    {
                        string character = "Player " + playerOffset;
                        string account = "Account " + (playerOffset++);
                        playerAgent.OverrideName(character + "\0:" + account + "\01");
                    }
                }
            }
            //
            _operation.UpdateProgressWithCancellationCheck("Creating GW2EI Combat Events");
            CombatData = new CombatData(combatItems, FightData, AgentData, SkillData, PlayerList, operation, extensions, evtcVersion);
            if (parserSettings.AnonymousPlayers)
            {
                operation.UpdateProgressWithCancellationCheck("Anonymous guilds");
                IReadOnlyList<AgentItem> allPlayerAgents = agentData.GetAgentByType(AgentItem.AgentType.Player);
                foreach (AgentItem playerAgent in allPlayerAgents)
                {
                    foreach (GuildEvent guildEvent in CombatData.GetGuildEvents(playerAgent))
                    {
                        guildEvent.Anonymize();
                    }
                }
            }
            //
            operation.UpdateProgressWithCancellationCheck("Checking CM");
            FightData.SetEncounterMode(CombatData, AgentData);
            operation.UpdateProgressWithCancellationCheck("Setting Fight Name");
            FightData.SetFightName(CombatData, AgentData);
            //
            var friendlies = new List<AbstractSingleActor>();
            friendlies.AddRange(PlayerList);
            friendlies.AddRange(fightData.Logic.NonPlayerFriendlies);
            Friendlies = friendlies;
            FriendliesListBySpec = friendlies.GroupBy(x => x.Spec).ToDictionary(x => x.Key, x => x.ToList());
            FriendlyAgents = new HashSet<AgentItem>(Friendlies.Select(x => x.AgentItem));
            //
            _operation.UpdateProgressWithCancellationCheck("Checking Success");
            FightData.Logic.CheckSuccess(CombatData, AgentData, FightData, PlayerAgents);
            if (FightData.FightDuration <= ParserSettings.TooShortLimit)
            {
                throw new TooShortException(FightData.FightDuration, ParserSettings.TooShortLimit);
            }
            if (ParserSettings.SkipFailedTries && !FightData.Success)
            {
                throw new SkipException();
            }
            _operation.UpdateProgressWithCancellationCheck("Creating GW2EI Log Meta Data");
            LogData = new LogData(evtcVersion, CombatData, FightData.LogEnd - FightData.LogStart, playerList, extensions, operation);
            //
            _operation.UpdateProgressWithCancellationCheck("Creating Buff Container");
            Buffs = new BuffsContainer(CombatData, operation);
            _operation.UpdateProgressWithCancellationCheck("Creating Damage Modifier Container");
            DamageModifiers = new DamageModifiersContainer(CombatData, fightData.Logic.Mode, parserSettings);
            _operation.UpdateProgressWithCancellationCheck("Creating Mechanic Data");
            MechanicData = FightData.Logic.GetMechanicData();
            _operation.UpdateProgressWithCancellationCheck("Creating General Statistics Container");
            StatisticsHelper = new StatisticsHelper(CombatData, PlayerList, Buffs);
        }

        public void UpdateProgressWithCancellationCheck(string status)
        {
            _operation.UpdateProgressWithCancellationCheck(status);
        }

        private void AddToDictionary(AbstractSingleActor actor)
        {
            _agentToActorDictionary[actor.AgentItem] = actor;
            /*foreach (Minions minions in actor.GetMinions(this).Values)
            {
                foreach (NPC npc in minions.MinionList)
                {
                    AddToDictionary(npc);
                }
            }*/
        }

        private void InitActorDictionaries()
        {
            if (_agentToActorDictionary == null)
            {
                _operation.UpdateProgressWithCancellationCheck("Initializing Actor dictionary");
                _agentToActorDictionary = new Dictionary<AgentItem, AbstractSingleActor>();
                foreach (AbstractSingleActor p in Friendlies)
                {
                    AddToDictionary(p);
                }
                foreach (AbstractSingleActor npc in FightData.Logic.Hostiles)
                {
                    AddToDictionary(npc);
                }
            }
        }

        /// <summary>
        /// Find the corresponding actor, creates one otherwise
        /// </summary>
        /// <param name="agentItem"><see cref="AgentItem"/> to find an <see cref="AbstractSingleActor"/> for</param>
        /// <param name="excludePlayers">returns null if true and agentItem is a player or has a player master</param>
        /// <returns></returns>
        public AbstractSingleActor FindActor(AgentItem agentItem, bool excludePlayers = false)
        {
            if (agentItem == null || (excludePlayers && agentItem.GetFinalMaster().Type == AgentItem.AgentType.Player))
            {
                return null;
            }
            InitActorDictionaries();
            if (!_agentToActorDictionary.TryGetValue(agentItem, out AbstractSingleActor actor))
            {
                if (agentItem.Type == AgentItem.AgentType.Player)
                {
                    actor = new Player(agentItem, true);
                    _operation.UpdateProgressWithCancellationCheck("Found player " + actor.Character + " not in player list");
                } 
                else if (agentItem.Type == AgentItem.AgentType.NonSquadPlayer)
                {
                    actor = new PlayerNonSquad(agentItem);
                } 
                else
                {
                    actor = new NPC(agentItem);
                }
                _agentToActorDictionary[agentItem] = actor;
                //throw new EIException("Requested actor with id " + a.ID + " and name " + a.Name + " is missing");
            }
            return actor;
        }
    }
}
