using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class PointOfViewEvent : AbstractMetaDataEvent
    {
        public AgentItem PoV { get; }

        public PointOfViewEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, offset)
        {
            PoV = agentData.GetAgent(Data, evtcItem.LogTime);
        }

    }
}
