using System.Buffers.Binary;

namespace GW2EIEvtcParser.ParsedData;

public class GuildEvent : MetaDataEvent
{
    public AgentItem Src { get; protected set; }

    private readonly string _guildKey;

    private bool _anomymous { set; get; } = false;

    internal unsafe GuildEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem)
    {
        Src = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);

        var guid = stackalloc byte[16];

        //NOTE(Rennorb): Diagram from how it was assigned before:
        //  3 2 1 0 5 4 7 6 (source index)
        //  v v v v v v v v
        //  0 1 2 3 4 5 6 7 (destination index)

        var dstAgent_ = evtcItem.DstAgent;
        var dstAgent = (byte*)&dstAgent_;
        *(UInt32*)guid = BinaryPrimitives.ReverseEndianness(*(UInt32*)dstAgent);
        guid[4] = dstAgent[5];
        guid[5] = dstAgent[4];
        guid[6] = dstAgent[7];
        guid[7] = dstAgent[6];

        *(Int32*)(guid + sizeof(UInt64)) = evtcItem.Value;
        *(Int32*)(guid + sizeof(UInt64) + sizeof(Int32)) = evtcItem.BuffDmg;

        var _guid = new Span<byte>(guid, 16);

        // 8f55c4ee-09cc-4b0d-896c-d81e58be0042
        Span<char> strBuffer = stackalloc char[32 + 4];
        ParserHelper.AppendHexString(strBuffer[..8], _guid[..4]);
        strBuffer[8] = '-';
        ParserHelper.AppendHexString(strBuffer[9..13], _guid[4..6]);
        strBuffer[13] = '-';
        ParserHelper.AppendHexString(strBuffer[14..18], _guid[6..8]);
        strBuffer[18] = '-';
        ParserHelper.AppendHexString(strBuffer[19..23], _guid[8..10]);
        strBuffer[23] = '-';
        ParserHelper.AppendHexString(strBuffer[24..36], _guid[10..16]);

        _guildKey = new String(strBuffer);
    }

    internal void Anonymize()
    {
        _anomymous = true;
    }

    public string APIString => _anomymous ? "" : _guildKey;
}
