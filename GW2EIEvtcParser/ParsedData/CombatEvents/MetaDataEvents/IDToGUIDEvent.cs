using System;

namespace GW2EIEvtcParser.ParsedData;

public abstract class IDToGUIDEvent : AbstractMetaDataEvent
{

    public string HexContentGUID { get; }
    public string Base64ContentGUID { get; }

    public long ContentID { get; }

    internal IDToGUIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
        (HexContentGUID, Base64ContentGUID) = UnpackGUID(evtcItem.SrcAgent, evtcItem.DstAgent);
        ContentID = evtcItem.SkillID;
    }

    internal IDToGUIDEvent() : base()
    {
        (HexContentGUID, Base64ContentGUID) = ("", "");
        ContentID = -1;
    }

    internal static unsafe (string hex, string base64) UnpackGUID(ulong first8, ulong last8)
    {
        Span<byte> guid = stackalloc byte[16];
        fixed(byte* ptr = guid)
        {
            *(UInt64*)ptr = first8;
            *(((UInt64*)ptr) + 1) = last8;
        }
        return (ParserHelper.ToHexString(guid), Convert.ToBase64String(guid));
    }

}
