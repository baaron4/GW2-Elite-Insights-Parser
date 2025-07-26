using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser;

public class ParsedEvtcLog
{
    public readonly LogData LogData;
    public readonly FightData FightData;
    public readonly AgentData AgentData;
    public readonly SkillData SkillData;
    public readonly CombatData CombatData;
    public readonly IReadOnlyList<Player> PlayerList;
    public readonly IReadOnlyList<SingleActor> Friendlies;
    public readonly IReadOnlyCollection<AgentItem> PlayerAgents;
    public readonly IReadOnlyCollection<AgentItem> FriendlyAgents;
    public bool IsBenchmarkMode => FightData.Logic.ParseMode == FightLogic.ParseModeEnum.Benchmark;
    public readonly IReadOnlyDictionary<ParserHelper.Spec, IReadOnlyList<SingleActor>> FriendliesListBySpec;
    public readonly DamageModifiersContainer DamageModifiers;
    public readonly BuffsContainer Buffs;
    public readonly EvtcParserSettings ParserSettings;
    public bool CanCombatReplay => ParserSettings.ParseCombatReplay && CombatData.HasMovementData;

    public readonly MechanicData MechanicData;
    public readonly StatisticsHelper StatisticsHelper;

    private readonly ParserController _operation;

    private Dictionary<AgentItem, SingleActor>? _agentToActorDictionary;

    internal ParsedEvtcLog(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, SkillData skillData,
            IReadOnlyList<CombatItem> combatItems, IReadOnlyList<Player> playerList, IReadOnlyDictionary<uint, ExtensionHandler> extensions, EvtcParserSettings parserSettings, ParserController operation)
    {
        FightData = fightData;
        AgentData = agentData;
        SkillData = skillData;
        ParserSettings = parserSettings;
        _operation = operation;
        
        _operation.UpdateProgressWithCancellationCheck("Parsing: Creating GW2EI Combat Events");
        CombatData = new CombatData(combatItems, FightData, AgentData, SkillData, playerList, operation, extensions, evtcVersion, parserSettings);
        
        operation.UpdateProgressWithCancellationCheck("Parsing: Checking Encounter Status");
        FightData.ProcessEncounterStatus(CombatData, AgentData);

        operation.UpdateProgressWithCancellationCheck("Parsing: Setting Fight Name");
        FightData.CompleteFightName(CombatData, AgentData);
        
        _operation.UpdateProgressWithCancellationCheck("Parsing: Checking Success");
        FightData.Logic.CheckSuccess(CombatData, AgentData, FightData, agentData.GetAgentByType(AgentItem.AgentType.Player));
        if (FightData.FightDuration <= ParserSettings.TooShortLimit)
        {
            throw new TooShortException(FightData.FightDuration, ParserSettings.TooShortLimit);
        }
        if (ParserSettings.SkipFailedTries && !FightData.Success)
        {
            throw new SkipException();
        }
        
        _operation.UpdateProgressWithCancellationCheck("Parsing: Handling active players");
        List<Player> activePlayers = [];
        foreach (Player p in playerList)
        {
            if (p.FirstAware < FightData.FightEnd)
            {
                activePlayers.Add(p);
            } 
            else
            {
                operation.UpdateProgressWithCancellationCheck("Parsing: Removing player from player list (spawned after fight end)");
            }
        }
        if (activePlayers.Count == 0)
        {
            throw new EvtcAgentException("No valid players");
        }
        PlayerList = activePlayers.OrderBy(a => a.Group).ToList();
        PlayerAgents = new HashSet<AgentItem>(PlayerList.Select(x => x.AgentItem));
        
        _operation.UpdateProgressWithCancellationCheck("Parsing: Handling friendlies");
        var friendlies = new List<SingleActor>();
        friendlies.AddRange(PlayerList);
        friendlies.AddRange(fightData.Logic.NonSquadFriendlies);
        Friendlies = friendlies;
        FriendliesListBySpec = friendlies.GroupBy(x => x.Spec).ToDictionary(x => x.Key, x => (IReadOnlyList<SingleActor>)x.ToList());
        FriendlyAgents = new HashSet<AgentItem>(Friendlies.Select(x => x.AgentItem));
        
        _operation.UpdateProgressWithCancellationCheck("Parsing: Player count: " + PlayerList.Count);
        _operation.UpdateProgressWithCancellationCheck("Parsing: Friendlies count: " + FightData.Logic.NonSquadFriendlies.Count);
        _operation.UpdateProgressWithCancellationCheck("Parsing: Targets count: " + FightData.Logic.Targets.Count);
        _operation.UpdateProgressWithCancellationCheck("Parsing: Trash Mobs count: " + FightData.Logic.TrashMobs.Count);
        
        _operation.UpdateProgressWithCancellationCheck("Parsing: Creating GW2EI Log Meta Data");
        LogData = new LogData(evtcVersion, CombatData, FightData.LogEnd - FightData.LogStart, playerList, extensions, operation);
        
        _operation.UpdateProgressWithCancellationCheck("Parsing: Creating Buff Container");
        Buffs = new BuffsContainer(CombatData, operation);

        _operation.UpdateProgressWithCancellationCheck("Parsing: Creating Damage Modifier Container");
        DamageModifiers = new DamageModifiersContainer(CombatData, fightData.Logic.ParseMode, fightData.Logic.SkillMode, parserSettings);

        _operation.UpdateProgressWithCancellationCheck("Parsing: Creating Mechanic Data");
        MechanicData = FightData.Logic.GetMechanicData();

        _operation.UpdateProgressWithCancellationCheck("Parsing: Creating General Statistics Container");
        StatisticsHelper = new StatisticsHelper(CombatData, PlayerList, Buffs);
    }

    public void UpdateProgressWithCancellationCheck(string status)
    {
        _operation.UpdateProgressWithCancellationCheck(status);
    }

    private void AddToDictionary(SingleActor actor)
    {
        _agentToActorDictionary![actor.AgentItem] = actor;
        /*foreach (Minions minions in actor.GetMinions(this).Values)
        {
            foreach (NPC npc in minions.MinionList)
            {
                AddToDictionary(npc);
            }
        }*/
    }
    [MemberNotNull(nameof(_agentToActorDictionary))]
    private void InitActorDictionaries()
    {
        if (_agentToActorDictionary == null)
        {
            _operation.UpdateProgressWithCancellationCheck("Parsing: Initializing Actor dictionary");
            _agentToActorDictionary = [];
            foreach (SingleActor p in Friendlies)
            {
                AddToDictionary(p);
            }
            foreach (SingleActor npc in FightData.Logic.Hostiles)
            {
                AddToDictionary(npc);
            }
        }
    }

    /// <summary>
    /// Find the corresponding actor, creates one otherwise
    /// </summary>
    /// <param name="agentItem"><see cref="AgentItem"/> to find an <see cref="SingleActor"/> for</param>
    /// <returns></returns>
    public SingleActor FindActor(AgentItem agentItem)
    {
        InitActorDictionaries();
        if (!_agentToActorDictionary.TryGetValue(agentItem, out var actor))
        {
            if (agentItem.Type == AgentItem.AgentType.Player)
            {
                actor = new Player(agentItem, true);
                _operation.UpdateProgressWithCancellationCheck("Parsing: Found player " + actor.Character + " not in player list");
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

    /// <summary>
    /// Find the corresponding actor, creates one otherwise
    /// </summary>
    /// <param name="agentItem"><see cref="AgentItem"/> to find an <see cref="SingleActor"/> for</param>
    /// <param name="excludePlayers">returns null if true and agentItem is a player or has a player master</param>
    /// <returns></returns>
    public SingleActor? FindActor(AgentItem agentItem, bool excludePlayers)
    {
        if (excludePlayers && agentItem.GetFinalMaster().Type == AgentItem.AgentType.Player)
        {
            return null;
        }
        return FindActor(agentItem);
    }


    public (List<SingleActorCombatReplayDescription>,List<CombatReplayRenderingDescription>, List<CombatReplayDecorationMetadataDescription>) GetCombatReplayDescriptions(Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var map = FightData.Logic.GetCombatReplayMap(this);
        var actors = new List<SingleActorCombatReplayDescription>();
        var decorationRenderings = new List<CombatReplayRenderingDescription>();
        var decorationMetadata = new List<CombatReplayDecorationMetadataDescription>();
        var fromNonFriendliesSet = new HashSet<SingleActor>(FightData.Logic.Hostiles);
        foreach (SingleActor actor in Friendlies)
        {
            if (actor.IsFakeActor || !actor.HasCombatReplayPositions(this))
            {
                continue;
            }
            actors.Add(actor.GetCombatReplayDescription(map, this));
            decorationRenderings.AddRange(actor.GetCombatReplayDecorationRenderableDescriptions(map, this, usedSkills, usedBuffs));
            foreach (Minions minions in actor.GetMinions(this).Values)
            {
                if (ParserHelper.IsKnownMinionID(minions.ReferenceAgentItem, actor.Spec))
                {
                    fromNonFriendliesSet.UnionWith(minions.MinionList);
                }
            }
        }
        foreach (SingleActor actor in fromNonFriendliesSet)
        {
            if ((actor.LastAware - actor.FirstAware < 200) || !actor.HasCombatReplayPositions(this))
            {
                continue;
            }
            actors.Add(actor.GetCombatReplayDescription(map, this));
            decorationRenderings.AddRange(actor.GetCombatReplayDecorationRenderableDescriptions(map, this, usedSkills, usedBuffs));

        }
        decorationRenderings.AddRange(FightData.Logic.GetCombatReplayDecorationRenderableDescriptions(map, this, usedSkills, usedBuffs));
        foreach (var pair in FightData.Logic.DecorationCache)
        {
            decorationMetadata.Add(pair.Value.GetCombatReplayMetadataDescription());
        }
        return (actors, decorationRenderings, decorationMetadata);
    }
}
