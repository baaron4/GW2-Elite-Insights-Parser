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
        public ParseEnum.StateChange StateChange => EvtcItem.IsStateChange;

        public AbstractMovementEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem)
        {
            AgentItem = agentData.GetAgentByInstID(evtcItem.SrcInstid, evtcItem.Time);
        }

        public (float x, float y, float z) Unpack()
        {
            byte[] xy = BitConverter.GetBytes(EvtcItem.DstAgent);
            float x = BitConverter.ToSingle(xy, 0);
            float y = BitConverter.ToSingle(xy, 4);

            return (x, y, EvtcItem.Value);
        }

        public abstract void AddPoint3D(CombatReplay replay, ParsedLog log);
    }
}
