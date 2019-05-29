using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractMovementEvent : AbstractCombatEvent
    {
        public AgentItem AgentItem { get; }
        private readonly ulong _dstAgent;
        private readonly long _value;

        public AbstractMovementEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem.Time, offset)
        {
            AgentItem = agentData.GetAgentByInstID(evtcItem.SrcInstid, evtcItem.Time);
            _dstAgent = evtcItem.DstAgent;
            _value = evtcItem.Value;
        }

        public (float x, float y, float z) Unpack()
        {
            byte[] xy = BitConverter.GetBytes(_dstAgent);
            float x = BitConverter.ToSingle(xy, 0);
            float y = BitConverter.ToSingle(xy, 4);

            return (x, y, _value);
        }

        public abstract void AddPoint3D(CombatReplay replay);
    }
}
