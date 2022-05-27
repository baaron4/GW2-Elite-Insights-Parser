using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class StatusMechanic<T> : Mechanic where T : AbstractStatusEvent
    {
        public delegate bool StatusChecker(T ce, ParsedEvtcLog log);

        public delegate IReadOnlyList<T> StatusGetter(ParsedEvtcLog log, AgentItem a);

        private readonly StatusChecker _triggerCondition = null;
        protected bool Keep(T c, ParsedEvtcLog log)
        {
            if (_triggerCondition != null)
            {
                return _triggerCondition(c, log);
            }
            return true;
        }

        private readonly StatusGetter _getter = null;

        public IReadOnlyList<T> GetEvents(ParsedEvtcLog log, AgentItem a)
        {
            return _getter(log, a);
        }

        public StatusMechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, StatusGetter getter, StatusChecker condition = null) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
            _getter = getter;
            if (_getter == null)
            {
                throw new InvalidOperationException("Missing getter in StatusMechanic");
            }
        }
    }
}
