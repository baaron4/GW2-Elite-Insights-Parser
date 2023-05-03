using System;

namespace GW2EIEvtcParser.ParsedData
{
    public class GuildEvent : AbstractMetaDataEvent
    {
        public AgentItem Src { get; protected set; }

        private readonly string _guildKey;

        private bool _anomymous { set; get; } = false;

        internal GuildEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem)
        {
            Src = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
            byte[] guid = new byte[16];
            byte[] first8 = BitConverter.GetBytes(evtcItem.DstAgent);
            byte[] mid4 = BitConverter.GetBytes(evtcItem.Value);
            byte[] last4 = BitConverter.GetBytes(evtcItem.BuffDmg);
            // The 4 bytes at the beginning has to be flipped
            guid[0] = first8[3];
            guid[1] = first8[2];
            guid[2] = first8[1];
            guid[3] = first8[0];
            guid[4] = first8[5];
            guid[5] = first8[4];
            guid[6] = first8[7];
            guid[7] = first8[6];
            //
            mid4.CopyTo(guid, first8.Length);
            last4.CopyTo(guid, first8.Length + mid4.Length);
            //
            _guildKey = ParserHelper.ToHexString(guid, 0, 4) + "-" + ParserHelper.ToHexString(guid, 4, 6) + "-" +
                ParserHelper.ToHexString(guid, 6, 8) + "-" + ParserHelper.ToHexString(guid, 8, 10) + "-" + ParserHelper.ToHexString(guid, 10, 16);
        }

        internal void Anonymize()
        {
            _anomymous = true;
        }

        public string APIString => _anomymous ? null : _guildKey;

    }
}
