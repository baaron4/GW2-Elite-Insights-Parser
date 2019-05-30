using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class RewardEvent : AbstractMetaDataEvent
    {
        public ulong RewardID { get; }
        public int RewardType { get; }

        public RewardEvent(CombatItem evtcItem, long offset) : base(evtcItem, offset)
        {
            RewardID = evtcItem.DstAgent;
            RewardType = evtcItem.Value;
        }

    }
}
