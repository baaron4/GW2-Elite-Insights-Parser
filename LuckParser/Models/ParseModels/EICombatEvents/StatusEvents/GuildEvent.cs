using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class GuildEvent : AbstractStatusEvent
    {
        public byte[] Guid { get; }

        public GuildEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            Guid = new byte[16];
            byte[] first8 = BitConverter.GetBytes(evtcItem.DstAgent);
            byte[] mid4 = BitConverter.GetBytes(evtcItem.Value);
            byte[] last4 = BitConverter.GetBytes(evtcItem.BuffDmg);
            Guid = new byte[first8.Length + mid4.Length + last4.Length];
            first8.CopyTo(Guid, 0);
            mid4.CopyTo(Guid, first8.Length);
            last4.CopyTo(Guid, first8.Length + mid4.Length);
        }

    }
}
