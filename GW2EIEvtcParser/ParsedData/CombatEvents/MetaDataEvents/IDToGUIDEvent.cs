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
            ContentGUID = UnpackGUID(evtcItem.SrcAgent, evtcItem.DstAgent);
            ContentID = evtcItem.SkillID;
        }

        internal static string UnpackGUID(ulong first8, ulong last8)
        {
            byte[] guid = new byte[16];
            byte[] first8Bytes = BitConverter.GetBytes(first8);
            byte[] last8Bytes = BitConverter.GetBytes(last8);
            first8Bytes.CopyTo(guid, 0);
            last8Bytes.CopyTo(guid, first8Bytes.Length);
            return ParserHelper.ToHexString(guid, 0, 16);
        }

    }
}
