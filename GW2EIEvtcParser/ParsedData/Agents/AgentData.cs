using System.Diagnostics.CodeAnalysis;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.ParsedData;

public class AgentData
{
#if DEBUG
    public readonly List<AgentItem> _allAgentsList;
#else
    private readonly List<AgentItem> _allAgentsList;
#endif
    private Dictionary<ulong, List<AgentItem>> _allAgentsByAgent;
    private Dictionary<ushort, List<AgentItem>> _allAgentsByInstID;
    private Dictionary<int, List<AgentItem>> _allNPCsByID;
    private Dictionary<int, List<AgentItem>> _allGadgetsByID;
    private Dictionary<AgentItem.AgentType, List<AgentItem>> _allAgentsByType;
    [Flags]
    internal enum AgentDataDirtyStatus
    {
        Clean = 0,
        SpeciesDirty = 1,
        TypesDirty = 2,
        AgentsDirty = 4,
        AllDirty = SpeciesDirty | TypesDirty | AgentsDirty
    }
    private AgentDataDirtyStatus _dirty = AgentDataDirtyStatus.AllDirty;
#if DEBUG
    private Dictionary<string, List<AgentItem>> _allAgentsByName;
#endif
    public IReadOnlyCollection<ulong> AgentValues => new HashSet<ulong>(_allAgentsList.Select(x => x.Agent));
    public IReadOnlyCollection<ushort> InstIDValues => new HashSet<ushort>(_allAgentsList.Select(x => x.InstID));


    private readonly GW2EIGW2API.GW2APIController _apiController;

    internal AgentData(GW2EIGW2API.GW2APIController apiController, List<AgentItem> allAgentsList)
    {
        _apiController = apiController;
        _allAgentsList = allAgentsList;
    }
    internal string GetSpec(uint prof, uint elite)
    {
        return _apiController.GetSpec(prof, elite);
    }

    internal AgentItem AddCustomNPCAgent(long start, long end, string name, ParserHelper.Spec spec, int ID, bool isFake, ushort toughness = 0, ushort healing = 0, ushort condition = 0, ushort concentration = 0, uint hitboxWidth = 0, uint hitboxHeight = 0)
    {
        var rnd = new Random();
        ulong agentValue = 0;
        while (AgentValues.Contains(agentValue) || agentValue == 0)
        {
            agentValue = (ulong)rnd.Next(int.MaxValue / 2, int.MaxValue);
        }
        ushort instID = 0;
        while (InstIDValues.Contains(instID) || instID == 0)
        {
            instID = (ushort)rnd.Next(ushort.MaxValue / 2, ushort.MaxValue);
        }
        var agent = new AgentItem(agentValue, name, spec, ID, AgentItem.AgentType.NPC, instID, toughness, healing, condition, concentration, hitboxWidth, hitboxHeight, start, end, isFake);
        _allAgentsList.Add(agent);
        _dirty |= AgentDataDirtyStatus.AgentsDirty;
        return agent;
    }

    internal AgentItem AddCustomNPCAgent(long start, long end, string name, ParserHelper.Spec spec, TargetID ID, bool isFake, ushort toughness = 0, ushort healing = 0, ushort condition = 0, ushort concentration = 0, uint hitboxWidth = 0, uint hitboxHeight = 0)
    {
        return AddCustomNPCAgent(start, end, name, spec, (int)ID, isFake, toughness, healing, condition, concentration, hitboxWidth, hitboxHeight);
    }

    internal AgentItem AddCustomAgentFrom(AgentItem from, long start, long end, ParserHelper.Spec spec)
    {
        var rnd = new Random();
        ulong agentValue = 0;
        while (AgentValues.Contains(agentValue) || agentValue == 0)
        {
            agentValue = (ulong)rnd.Next(int.MaxValue / 2, int.MaxValue);
        }
        ushort instID = 0;
        while (InstIDValues.Contains(instID) || instID == 0)
        {
            instID = (ushort)rnd.Next(ushort.MaxValue / 2, ushort.MaxValue);
        }
        var agent = new AgentItem(agentValue, from.Name, spec, 0, from.Type, instID, from.Toughness, from.Healing, from.Condition, from.Concentration, from.HitboxWidth, from.HitboxHeight, start, end, from.IsFake);
        _allAgentsList.Add(agent);
        _dirty |= AgentDataDirtyStatus.AgentsDirty;
        return agent;
    }

    public AgentItem GetAgent(ulong agentAddress, long time)
    {
        if (agentAddress != 0)
        {
            if ((_dirty & AgentDataDirtyStatus.AgentsDirty) > 0)
            {
                Refresh();
            }
            if (_allAgentsByAgent.TryGetValue(agentAddress, out var agents))
            {
                foreach (AgentItem a in agents)
                {
                    if (a.InAwareTimes(time))
                    {
                        return a;
                    }
                }
            }
        }
        return ParserHelper._unknownAgent;
    }

    public IReadOnlyList<AgentItem> GetNPCsByID(int id)
    {
        if ((_dirty & AgentDataDirtyStatus.AllDirty) > 0)
        {
            Refresh();
        }
        if (_allNPCsByID.TryGetValue(id, out var list))
        {
            return list;
        }
        return [];
    }
    public IReadOnlyList<AgentItem> GetNPCsByIDAndAgent(int id, ulong agent)
    {
        if (agent == 0)
        {
            return GetNPCsByID(id);
        }
        return GetNPCsByID(id).Where(x => x.Agent == agent).ToList();
    }

    public IReadOnlyList<AgentItem> GetNPCsByID(TargetID id)
    {
        return GetNPCsByID((int)id);
    }
    public IReadOnlyList<AgentItem> GetNPCsByIDs(TargetID[] ids)
    {
        var list = new List<AgentItem>();
        foreach (var id in ids)
        {
            list.AddRange(GetNPCsByID(id));
        }
        return list;
    }
    public IReadOnlyList<AgentItem> GetNPCsByIDAndAgent(TargetID id, ulong agent)
    {
        return GetNPCsByIDAndAgent((int)id, agent);
    }

    public IReadOnlyList<AgentItem> GetNPCsByID(MinionID id)
    {
        return GetNPCsByID((int)id);
    }
    public IReadOnlyList<AgentItem> GetNPCsByIDAndAgent(MinionID id, ulong agent)
    {
        return GetNPCsByIDAndAgent((int)id, agent);
    }


    public IReadOnlyList<AgentItem> GetGadgetsByID(int id)
    {
        if ((_dirty & AgentDataDirtyStatus.AllDirty) > 0)
        {
            Refresh();
        }
        if (_allGadgetsByID.TryGetValue(id, out var list))
        {
            return list;
        }
        return new List<AgentItem>();
    }

    public IReadOnlyList<AgentItem> GetGadgetsByID(TargetID id)
    {
        return GetGadgetsByID((int)id);
    }
    public IReadOnlyList<AgentItem> GetGadgetsByID(MinionID id)
    {
        return GetGadgetsByID((int)id);
    }

    public IReadOnlyList<AgentItem> GetGadgetsByID(ChestID id)
    {
        return GetGadgetsByID((int)id);
    }


    public AgentItem GetAgentByUniqueID(long uniqueID)
    {
        return _allAgentsList.FirstOrDefault(x => x.UniqueID == uniqueID) ?? ParserHelper._unknownAgent;
    }

    public AgentItem GetAgentByInstID(ushort instid, long time)
    {
        if (instid != 0)
        {
            if ((_dirty & AgentDataDirtyStatus.AgentsDirty) > 0)
            {
                Refresh();
            }
            if (_allAgentsByInstID.TryGetValue(instid, out var agents))
            {
                foreach (AgentItem a in agents)
                {
                    if (a.InAwareTimes(time))
                    {
                        return a;
                    }
                }
            }
        }
        return ParserHelper._unknownAgent;
    }

    public bool HasSpawnedMinion(MinionID minion, AgentItem master, long time, long epsilon = ParserHelper.ServerDelayConstant)
    {
        return GetNPCsByID(minion)
            .Any(agent => master.IsMasterOf(agent) && Math.Abs(agent.FirstAware - time) < epsilon);
    }

    internal void ReplaceAgents(IEnumerable<AgentItem> toRemove, IEnumerable<AgentItem> toAdd)
    {
        if (toAdd.Any())
        {
            _allAgentsList.RemoveAll(toRemove.Contains);
            _allAgentsList.AddRange(toAdd);
            _dirty |= AgentDataDirtyStatus.AgentsDirty;
        }
    }

    internal void FlagAsDirty(AgentDataDirtyStatus status)
    {
        _dirty |= status;
    }

    internal void ApplyOffset(long offset)
    {
        foreach (var agentItem in _allAgentsList)
        {
            agentItem.ApplyOffset(offset);
        }
    }

    internal void RemoveAllFrom(HashSet<AgentItem> agents)
    {
        if (agents.Count == 0)
        {
            return;
        }
        _allAgentsList.RemoveAll(x => agents.Contains(x));
        _dirty |= AgentDataDirtyStatus.AgentsDirty;
    }

    private void Refresh()
    {
        var notEnglobingAgents = _allAgentsList.Where(x => !x.IsEnglobingAgent);
        _allAgentsByAgent = _allAgentsList.GroupBy(x => x.Agent).ToDictionary(x => x.Key, x => x.ToList());
        _allNPCsByID = notEnglobingAgents.Where(x => x.Type == AgentItem.AgentType.NPC).GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        _allGadgetsByID = notEnglobingAgents.Where(x => x.Type == AgentItem.AgentType.Gadget).GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        _allAgentsByInstID = _allAgentsList.GroupBy(x => x.InstID).ToDictionary(x => x.Key, x => x.ToList());
        _allAgentsByType = notEnglobingAgents.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
#if DEBUG
        _allAgentsByName = notEnglobingAgents.Where(x => !x.Name.Contains("UNKNOWN")).GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList());
#endif
        _dirty = AgentDataDirtyStatus.Clean;
    }

    public IReadOnlyList<AgentItem> GetAgentByType(AgentItem.AgentType type)
    {
        if ((_dirty & AgentDataDirtyStatus.AllDirty) > 0)
        {
            Refresh();
        }
        if (_allAgentsByType.TryGetValue(type, out var list))
        {
            return list;
        }
        else
        {
            return new List<AgentItem>();
        }

    }

    public delegate long AgentGroupingTimeFetchet(AgentItem agentItem);

    public static IEnumerable<IEnumerable<AgentItem>> GetGroupedAgentsByTimeCondition(IEnumerable<AgentItem> agents, AgentGroupingTimeFetchet timeFetcher, long epsilon = ParserHelper.ServerDelayConstant)
    {
        var groupedAgents = new List<IEnumerable<AgentItem>>();
        var processedTimes = new HashSet<long>();
        foreach (var agent in agents)
        {
            long time = timeFetcher(agent);
            if (processedTimes.Contains(time))
            {
                continue;
            }
            var group = agents.Where(otherAgent => timeFetcher(otherAgent) >= time && timeFetcher(otherAgent) < time + epsilon);
            foreach (var groupedAgent in group)
            {
                processedTimes.Add(timeFetcher(groupedAgent));
            }

            groupedAgents.Add(group);
        }
        return groupedAgents;
    }

    internal void SwapMasters(HashSet<AgentItem> froms, AgentItem to)
    {
        foreach (AgentItem a in GetAgentByType(AgentItem.AgentType.NPC))
        {
            if (a.Master != null && froms.Any(a.Is))
            {
                a.SetMaster(to);
            }
        }
        foreach (AgentItem a in GetAgentByType(AgentItem.AgentType.Gadget))
        {
            if (a.Master != null && froms.Any(a.Is))
            {
                a.SetMaster(to);
            }
        }
    }

    internal void SwapMasters(AgentItem from, AgentItem to)
    {
        SwapMasters([from], to);
    }

    /// <summary>
    /// Tries to retrieve the first <see cref="AgentItem"/> corresponding to the provided <see cref="TargetID"/>.
    /// </summary>
    /// <param name="targetID">The ID of the target to search for.</param>
    /// <param name="agentItem">The <see cref="AgentItem"/> found, if any.</param>
    /// <returns><see langword="true"/> if an <see cref="AgentItem"/> was found for the given  <see cref="TargetID"/>; otherwise,  <see langword="false"/>.</returns>
    public bool TryGetFirstAgentItem(TargetID targetID, [NotNullWhen(returnValue: true)] out AgentItem? agentItem)
    {
        return TryGetFirstAgentItem((int)targetID, out agentItem);
    }

    /// <summary>
    /// Tries to retrieve the first <see cref="AgentItem"/> corresponding to the provided <see cref="MinionID"/>.
    /// </summary>
    /// <param name="minionID">The ID of the minion to search for.</param>
    /// <param name="agentItem">The <see cref="AgentItem"/> found, if any.</param>
    /// <returns><see langword="true"/> if an <see cref="AgentItem"/> was found for the given <see cref="MinionID"/>; otherwise,  <see langword="false"/>.</returns>
    public bool TryGetFirstAgentItem(MinionID minionID, [NotNullWhen(returnValue: true)] out AgentItem? agentItem)
    {
        return TryGetFirstAgentItem((int)minionID, out agentItem);
    }

    /// <summary>
    /// Tries to retrieve the first <see cref="AgentItem"/> corresponding to the provided <see cref="ChestID"/>.
    /// </summary>
    /// <param name="chestID">The ID of the chest to search for.</param>
    /// <param name="agentItem">The <see cref="AgentItem"/> found, if any.</param>
    /// <returns><see langword="true"/> if an <see cref="AgentItem"/> was found for the given <see cref="ChestID"/>; otherwise,  <see langword="false"/>.</returns>
    public bool TryGetFirstAgentItem(ChestID chestID, [NotNullWhen(returnValue: true)] out AgentItem? agentItem)
    {
        return TryGetFirstAgentItem((int)chestID, out agentItem);
    }

    /// <summary>
    /// Tries to retrieve the first <see cref="AgentItem"/> corresponding to the provided <paramref name="speciesID"/>.<br></br>
    /// </summary>
    /// <param name="speciesID">The ID of the agent to search for.</param>
    /// <param name="agentItem">The <see cref="AgentItem"/> found, if any.</param>
    /// <returns><see langword="true"/> if an <see cref="AgentItem"/> was found for the given <paramref name="speciesID"/>; otherwise, <see langword="false"/>.</returns>
    public bool TryGetFirstAgentItem(int speciesID, [NotNullWhen(returnValue: true)] out AgentItem? agentItem)
    {
        agentItem = GetNPCsByID(speciesID).FirstOrDefault();
        if (agentItem != null)
        {
            return true;
        }
        return false;
    }
}
