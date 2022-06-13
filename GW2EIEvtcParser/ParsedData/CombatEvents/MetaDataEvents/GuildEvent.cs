using System;

namespace GW2EIEvtcParser.ParsedData
{
    public class GuildEvent : AbstractMetaDataEvent
    {
        public AgentItem Src { get; protected set; }

        private byte[] Guid { get; }

        private bool _anomymous { set; get; } = false;

        internal GuildEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem)
        {
            Src = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
            Guid = new byte[16];
            byte[] first8 = BitConverter.GetBytes(evtcItem.DstAgent);
            byte[] mid4 = BitConverter.GetBytes(evtcItem.Value);
            byte[] last4 = BitConverter.GetBytes(evtcItem.BuffDmg);
            // The 4 bytes at the beginning has to be flipped
            Guid[0] = first8[3];
            Guid[1] = first8[2];
            Guid[2] = first8[1];
            Guid[3] = first8[0];
            Guid[4] = first8[5];
            Guid[5] = first8[4];
            Guid[6] = first8[7];
            Guid[7] = first8[6];
            //
            mid4.CopyTo(Guid, first8.Length);
            last4.CopyTo(Guid, first8.Length + mid4.Length);
        }

        internal void Anonymize()
        {
            _anomymous = true;
        }

        private string ToHexString(int start, int end)
        {
            string res = "";
            for(int i = start; i < end; i++)
            {
                res += Guid[i].ToString("X2");
            }
            return res;
        }

        public string APIString => _anomymous ? null : ToHexString(0, 4) + "-" + ToHexString(4, 6) + "-" +
                ToHexString(6, 8) + "-" + ToHexString(8, 10) + "-" + ToHexString(10, 16);

    }
}
