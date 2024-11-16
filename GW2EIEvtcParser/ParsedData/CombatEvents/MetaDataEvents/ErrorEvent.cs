using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser.ParsedData;

public class ErrorEvent : MetaDataEvent
{
    public readonly string Message;
    internal ErrorEvent(CombatItem evtcItem) : base(evtcItem)
    {
        var bytes = new ByteBuffer(stackalloc byte[32]); // 32 * sizeof(char), char as in C not C#
        // 8 bytes
        bytes.PushNative(evtcItem.Time);
        // 8 bytes
        bytes.PushNative(evtcItem.SrcAgent);
        // 8 bytes
        bytes.PushNative(evtcItem.DstAgent);
        // 4 bytes
        bytes.PushNative(evtcItem.Value);
        // 4 bytes
        bytes.PushNative(evtcItem.BuffDmg);
        Message = System.Text.Encoding.UTF8.GetString(bytes);
    }

    internal ErrorEvent(string message)
    {
        Message = message;
    }
}
