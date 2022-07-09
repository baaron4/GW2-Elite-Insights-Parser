using System;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class ContentIDToContentGUIDEvent : AbstractMetaDataEvent
    {

        public string ContentGuidKey { get; }

        public ContentLocal ContentType { get; }

        public long ContentID { get; }

        internal ContentIDToContentGUIDEvent(CombatItem evtcItem) : base(evtcItem)
        {
            var guid = new byte[16];
            byte[] first8 = BitConverter.GetBytes(evtcItem.SrcAgent);
            byte[] last8 = BitConverter.GetBytes(evtcItem.DstAgent);
            first8.CopyTo(guid, 0);
            last8.CopyTo(guid, first8.Length);
            ContentGuidKey = ParserHelper.ToHexString(guid, 0, 16);
            ContentType = GetContentLocal((byte)evtcItem.OverstackValue);
            ContentID = evtcItem.SkillID;
        }

    }
}
