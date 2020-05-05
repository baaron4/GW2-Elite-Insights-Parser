using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Exceptions;
using GW2EIParser.Logic;
#if DEBUG
using static GW2EIParser.Parser.ParsedData.CombatEvents.BuffDataEvent;
#endif

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
            _operation.UpdateProgress("Creating GW2EI Combat Events");
            CombatData = new CombatData(combatItems, FightData, AgentData, SkillData, playerList);
            _operation.UpdateProgress("Creating GW2EI Log Meta Data");
            LogData = new LogData(buildVersion, CombatData, evtcLogDuration);
            _operation.UpdateProgress("GW2 Build " + LogData.GW2Version);
            //
            _operation.UpdateProgress("Checking Success");
            FightData.Logic.CheckSuccess(CombatData, AgentData, FightData, PlayerAgents);
            if (FightData.FightEnd <= 2200)
            {
                throw new TooShortException();
            }
            if (ParserSettings.SkipFailedTries && !FightData.Success)
            {
                throw new SkipException();
            }
            _operation.UpdateProgress("Checking CM");
            FightData.SetCM(CombatData, AgentData, FightData);
            //
            _operation.UpdateProgress("Creating Buff Container");
            Buffs = new BuffsContainer(LogData.GW2Version);
#if DEBUG
            var seenUnknowns = new HashSet<byte>();
            foreach (Buff buff in Buffs.AllBuffs)
            {
                CombatEvents.BuffDataEvent buffDataEvt = CombatData.GetBuffDataEvent(buff.ID);
                if (buffDataEvt != null)
                {
                    foreach (BuffFormula formula in buffDataEvt.FormulaList)
                    {
                        if (formula.Attr1 == ParseEnum.BuffAttribute.Unknown && !seenUnknowns.Contains(formula.DebugAttr1))
                        {
                            seenUnknowns.Add(formula.DebugAttr1);
                            operation.UpdateProgress("Unknown Formula " + formula.DebugAttr1 + " for " + buff.ID + " " + buff.Name);
                        }
                    }
                }
            }
#endif
            _operation.UpdateProgress("Creating Damage Modifier Container");
            DamageModifiers = new DamageModifiersContainer(LogData.GW2Version);
            _operation.UpdateProgress("Creating Mechanic Data");
            MechanicData = FightData.Logic.GetMechanicData();
            _operation.UpdateProgress("Creating General Statistics Container");
            Statistics = new GeneralStatistics(CombatData, PlayerList, Buffs);
        }

        public void UpdateProgress(string status)
        {
            _operation.UpdateProgress(status);
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
                _operation.UpdateProgress("Initializing Actor dictionary");
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
