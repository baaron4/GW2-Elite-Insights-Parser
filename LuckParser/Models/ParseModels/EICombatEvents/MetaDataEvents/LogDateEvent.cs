using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class LogDateEvent : AbstractMetaDataEvent
    {
        public uint ServerUnixTimeStamp { get; }
        public uint LocalUnixTimeStamp { get; }

        public LogDateEvent(CombatItem evtcItem, long offset) : base(evtcItem, offset)
        {
            ServerUnixTimeStamp = (uint)evtcItem.Value;
            LocalUnixTimeStamp = (uint)evtcItem.BuffDmg;
        }

    }
}
