using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractMetaDataEvent : AbstractCombatEvent
    {
        public ulong Data { get; }

        public AbstractMetaDataEvent(CombatItem evtcItem, long offset) : base(evtcItem.LogTime, offset)
        {
            Data = evtcItem.SrcAgent;
        }

    }
}
