using System;
using System.Collections.Generic;

namespace GW2EIEvtcParser.ParsedData
{
    public class EvtcVersionEvent : AbstractMetaDataEvent
    {
        public int Build { get; }
        public int Revision { get; } = -1;
        internal EvtcVersionEvent(CombatItem evtcItem) : base(evtcItem)
        {
            var bytes = new List<byte>();
            // 8 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.SrcAgent))
            {
                bytes.Add(bt);
            }
            // 8 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.DstAgent))
            {
                bytes.Add(bt);
            }
            // 4 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.Value))
            {
                bytes.Add(bt);
            }
            // 4 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.BuffDmg))
            {
                bytes.Add(bt);
            }
            // 4 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.OverstackValue))
            {
                bytes.Add(bt);
            }
            // 4 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.SkillID))
            {
                bytes.Add(bt);
            }
            // 2 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.SrcInstid))
            {
                bytes.Add(bt);
            }
            // 2 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.DstInstid))
            {
                bytes.Add(bt);
            }
            // 2 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.SrcMasterInstid))
            {
                bytes.Add(bt);
            }
            // 2 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.DstMasterInstid))
            {
                bytes.Add(bt);
            }
            // 1 byte
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IFFByte))
            {
                bytes.Add(bt);
            }
            // 1 byte
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsBuff))
            {
                bytes.Add(bt);
            }
            // 1 byte
            foreach (byte bt in BitConverter.GetBytes(evtcItem.Result))
            {
                bytes.Add(bt);
            }
            // 1 byte
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsActivationByte))
            {
                bytes.Add(bt);
            }
            // 1 byte
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsBuffRemoveByte))
            {
                bytes.Add(bt);
            }
            // 1 byte
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsNinety))
            {
                bytes.Add(bt);
            }
            // 1 byte
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsFifty))
            {
                bytes.Add(bt);
            }
            // 1 byte
            foreach (byte bt in BitConverter.GetBytes(evtcItem.IsMoving))
            {
                bytes.Add(bt);
            }
            string evtcVersion = System.Text.Encoding.UTF8.GetString(bytes.ToArray()).TrimEnd('\0');
            var majorSplit = evtcVersion.Split('.');
            Build = int.Parse(majorSplit[0]);
            var minorSplit = majorSplit[1].Split('-');
            Revision = int.Parse(minorSplit[0]);
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
}
