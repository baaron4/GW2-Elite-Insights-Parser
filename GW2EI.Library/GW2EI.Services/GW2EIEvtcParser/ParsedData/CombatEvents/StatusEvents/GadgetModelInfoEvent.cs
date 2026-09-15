using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser.ParsedData;

public class GadgetModelInfoEvent : StatusEvent
{
    public readonly byte HidingBits;

    public readonly string Model;

    internal GadgetModelInfoEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        HidingBits = evtcItem.IsFlanking;
        try
        {
            var bytes = new ByteBuffer(stackalloc byte[40]);
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

            Model = System.Text.Encoding.UTF8.GetString(bytes).TrimEnd('\0');
        } 
        catch (Exception) 
        {
            Model = string.Empty;
        }
    }

}
