using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser.ParsedData;

public class EvtcVersionEvent : MetaDataEvent
{
    public readonly int Build;
    public readonly int Revision = -1;
    internal EvtcVersionEvent(CombatItem evtcItem) : base(evtcItem)
    {
        var bytes = new ByteBuffer(stackalloc byte[48]);
        // 8 bytes
        bytes.PushNative(evtcItem.SrcAgent);
        // 8 bytes
        bytes.PushNative(evtcItem.DstAgent);
        // 4 bytes
        bytes.PushNative(evtcItem.Value);
        // 4 bytes
        bytes.PushNative(evtcItem.BuffDmg);
        // 4 bytes
        bytes.PushNative(evtcItem.OverstackValue);
        // 4 bytes
        bytes.PushNative(evtcItem.SkillID);
        // 2 bytes
        bytes.PushNative(evtcItem.SrcInstid);
        // 2 bytes
        bytes.PushNative(evtcItem.DstInstid);
        // 2 bytes
        bytes.PushNative(evtcItem.SrcMasterInstid);
        // 2 bytes
        bytes.PushNative(evtcItem.DstMasterInstid);
        // 1 byte
        bytes.PushNative(evtcItem.IFFByte);
        // 1 byte
        bytes.PushNative(evtcItem.IsBuff);
        // 1 byte
        bytes.PushNative(evtcItem.Result);
        // 1 byte
        bytes.PushNative(evtcItem.IsActivationByte);
        // 1 byte
        bytes.PushNative(evtcItem.IsBuffRemoveByte);
        // 1 byte
        bytes.PushNative(evtcItem.IsNinety);
        // 1 byte
        bytes.PushNative(evtcItem.IsFifty);
        // 1 byte
        bytes.PushNative(evtcItem.IsMoving);

        string evtcVersion = System.Text.Encoding.UTF8.GetString(bytes).TrimEnd('\0');
        var majorSplit = StringExt.SplitOnce(evtcVersion, '.');
        Build = int.Parse(majorSplit.Tail);
        var minorSplit = majorSplit.Head.SplitOnce('-');
        Revision = int.Parse(minorSplit.Tail);
    }
    internal EvtcVersionEvent(int version)
    {
        Build = version;
    }

    internal string ToEVTCString(bool buildOnly)
    {
        var start = "EVTC";
        if (Revision > -1 && !buildOnly)
        {
            return start + Build + "." + Revision;
        }
        else
        {
            return start + Build;
        }
    }
}
