using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class CastEndEvent : AbstractCastEvent
    {
        public CastEndEvent(CombatItem evtcItem, long offset) : base(evtcItem, offset)
        {

        }
    }
}
