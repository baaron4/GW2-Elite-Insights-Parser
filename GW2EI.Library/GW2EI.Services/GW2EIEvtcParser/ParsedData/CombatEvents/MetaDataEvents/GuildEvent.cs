namespace GW2EIEvtcParser.ParsedData;

public class GuildEvent : MetaDataEvent
{
    public AgentItem Src { get; protected set; }

    private readonly Guid _guildGuid;

    private bool _anomymous { set; get; } = false;

    internal unsafe GuildEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem)
    {
        Src = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);

        var guid = stackalloc byte[8];

        *(Int32*)(guid) = evtcItem.Value;
        *(Int32*)(guid + sizeof(Int32)) = evtcItem.BuffDmg;

        _guildGuid = new GUIDWrapper(evtcItem.DstAgent, *(UInt64*)(guid), false).GUID;

    }

    internal void Anonymize()
    {
        _anomymous = true;
    }

    public string APIString => _anomymous ? "" : _guildGuid.ToString().ToUpperInvariant();
}
