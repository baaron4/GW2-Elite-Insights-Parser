using System;

namespace GW2EIEvtcParser.ParsedData
{
    public class EffectIDToGUIDEvent : AbstractMetaDataEvent
    {

        private byte[] Guid { get; }

        public string GuidKey { get; }

        public uint ContentType { get; }

        public long EffectID { get; }

        internal EffectIDToGUIDEvent(CombatItem evtcItem) : base(evtcItem)
        {
            Guid = new byte[16];
            byte[] first8 = BitConverter.GetBytes(evtcItem.SrcAgent);
            byte[] last8 = BitConverter.GetBytes(evtcItem.DstAgent);
            first8.CopyTo(Guid, 0);
            last8.CopyTo(Guid, first8.Length);
            GuidKey = evtcItem.SrcAgent + "-" + evtcItem.DstAgent;
            ContentType = evtcItem.OverstackValue;
            EffectID = evtcItem.SkillID;
        }

    }
}
