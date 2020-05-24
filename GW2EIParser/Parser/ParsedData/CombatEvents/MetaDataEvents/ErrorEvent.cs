using System;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class ErrorEvent : AbstractMetaDataEvent
    {
        public string Message { get; }
        public ErrorEvent(CombatItem evtcItem) : base(evtcItem)
        {
            byte[] bytes = new byte[32 * sizeof(char)];
            int offset = 0;
            foreach (byte bt in BitConverter.GetBytes(evtcItem.Time))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.SrcAgent))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.DstAgent))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.Value))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.BuffDmg))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.OverstackValue))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.SkillID))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.SrcInstid))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.DstInstid))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.SrcMasterInstid))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.DstMasterInstid))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes((byte)evtcItem.IFFEnum))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsBuff))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes((byte)evtcItem.IFFEnum))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsBuff))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.Result))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsActivation))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsBuffRemove))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsNinety))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsFifty))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsMoving))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsStateChange))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsFlanking))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsShields))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsOffcycle))
            {
                bytes[offset++] = bt;
            }
            foreach (byte bt in BitConverter.GetBytes(evtcItem.Pad))
            {
                bytes[offset++] = bt;
            }
            Message = System.Text.Encoding.UTF8.GetString(bytes);
        }

    }
}
