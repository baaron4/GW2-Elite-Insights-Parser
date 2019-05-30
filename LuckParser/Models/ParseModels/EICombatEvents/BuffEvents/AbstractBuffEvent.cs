using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractBuffEvent : AbstractCombatEvent
    {
        public long Value { get; }
        public long BuffID { get; private set; }
        private long _originalBuffID;
        public AgentItem Src { get; protected set; }
        public AgentItem Dst { get; protected set; }

        public AbstractBuffEvent(CombatItem evtcItem, long offset) : base(evtcItem.Time, offset)
        {
            Value = evtcItem.Value;
            BuffID = evtcItem.SkillID;
        }

        public void Invalidate()
        {
            if (BuffID != BoonHelper.NoBuff)
            {
                _originalBuffID = BuffID;
                BuffID = BoonHelper.NoBuff;
            }
        }

        public abstract void UpdateSimulator(BoonSimulator simulator);

        public abstract void TryFindSrc(ParsedLog log);

        public abstract bool IsBoonSimulatorCompliant();
    }
}
