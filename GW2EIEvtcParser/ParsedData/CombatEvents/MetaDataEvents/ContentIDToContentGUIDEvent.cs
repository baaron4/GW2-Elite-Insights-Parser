using System;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class ContentIDToContentGUIDEvent : AbstractMetaDataEvent
    {

        private byte[] ContentGuid { get; }

        public string ContentGuidKey { get; }

        public ContentLocal ContentType { get; }

        public long ContentID { get; }

        internal ContentIDToContentGUIDEvent(CombatItem evtcItem) : base(evtcItem)
        {
            ContentGuid = new byte[16];
            byte[] first8 = BitConverter.GetBytes(evtcItem.SrcAgent);
            byte[] last8 = BitConverter.GetBytes(evtcItem.DstAgent);
            first8.CopyTo(ContentGuid, 0);
            last8.CopyTo(ContentGuid, first8.Length);
            ContentGuidKey = evtcItem.SrcAgent + "-" + evtcItem.DstAgent;
            ContentType = GetContentLocal((byte)evtcItem.OverstackValue);
            ContentID = evtcItem.SkillID;
        }

    }
}
