using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLActors;


internal abstract class ActorDto
{
    public int UniqueID;
    public int InstID;
    public string Name;
    public uint Tough;
    public uint Condi;
    public uint Conc;
    public uint Heal;
    public string Icon;
    public long Health;
    public double FirstAware;
    public double LastAware;
    public bool IsEnglobed;
    public bool IsFirstEnglobed;
    public bool IsLastEnglobed;
    public List<MinionDto> Minions;
    public ActorDetailsDto Details;

    protected ActorDto(SingleActor actor, ParsedEvtcLog log, ActorDetailsDto details)
    {
        Health = actor.GetHealth(log.CombatData);
        Condi = actor.Condition;
        Conc = actor.Concentration;
        Heal = actor.Healing;
        Icon = actor.GetIcon();
        Name = actor.Character;
        Tough = actor.Toughness;
        Details = details;
        UniqueID = actor.UniqueID;
        InstID = actor.InstID;
        FirstAware = actor.FirstAware / 1000.0;
        LastAware = actor.LastAware / 1000.0;
        IsEnglobed = actor.AgentItem.IsEnglobedAgent;
        if (IsEnglobed)
        {
            IsFirstEnglobed = actor.EnglobingAgentItem.EnglobedAgentItems.First() == actor.AgentItem;
            IsLastEnglobed = actor.EnglobingAgentItem.EnglobedAgentItems.Last() == actor.AgentItem;
        }
        var minions = actor.GetMinions(log);
        Minions = new(minions.Count);
        foreach (KeyValuePair<long, Minions> pair in minions)
        {
            Minions.Add(new MinionDto()
            {
                Id = pair.Key,
                Name = pair.Value.Character
            });
        }
    }
}
