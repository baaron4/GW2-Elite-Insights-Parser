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
        public long BuffID { get; private set; }
        private long _originalBuffID;
        public AgentItem By { get; protected set; }
        public AgentItem To { get; protected set; }

        public AbstractBuffEvent(CombatItem evtcItem, long offset) : base(evtcItem.LogTime, offset)
        {
            BuffID = evtcItem.SkillID;
        }

        public AbstractBuffEvent(long buffID, long time) : base(time, 0)
        {
            BuffID = buffID;
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

        public abstract bool IsBoonSimulatorCompliant(long fightEnd);
    }
}
