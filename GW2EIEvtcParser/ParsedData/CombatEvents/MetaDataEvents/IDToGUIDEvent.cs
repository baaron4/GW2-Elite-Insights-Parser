using System;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class IDToGUIDEvent : AbstractMetaDataEvent
    {

        public string ContentGUID { get; }

        public long ContentID { get; }

        internal IDToGUIDEvent(CombatItem evtcItem) : base(evtcItem)
        {
            var guid = new byte[16];
            byte[] first8 = BitConverter.GetBytes(evtcItem.SrcAgent);
            byte[] last8 = BitConverter.GetBytes(evtcItem.DstAgent);
            first8.CopyTo(guid, 0);
            last8.CopyTo(guid, first8.Length);
            ContentGUID = ParserHelper.ToHexString(guid, 0, 16);
            ContentID = evtcItem.SkillID;
        }

    }
}
