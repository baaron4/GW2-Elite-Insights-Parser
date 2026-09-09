using GW2EIParserAvalonia.Models;

namespace GW2EIParserAvalonia.Models;

public sealed class AgentFilterItem
{
    public ulong Agent { get; }
    public ushort InstID { get; }
    public int ID { get; }
    public string Name { get; }

    public string DisplayName => string.IsNullOrWhiteSpace(Name) ? $"Agent {Agent:X}" : $"{Name} [{InstID}]";

    public AgentFilterItem(AgentDataModel agent)
    {
        Agent = agent.Agent;
        InstID = agent.InstID;
        ID = agent.ID;
        Name = agent.Name;
    }
}
