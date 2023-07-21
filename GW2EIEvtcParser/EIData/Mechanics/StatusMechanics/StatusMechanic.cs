using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class StatusMechanic<T> : CheckedMechanic<T> where T : AbstractStatusEvent
    {

        public delegate IReadOnlyList<T> StatusGetter(ParsedEvtcLog log, AgentItem agent);

        private readonly StatusGetter _getter = null;

        public IReadOnlyList<T> GetEvents(ParsedEvtcLog log, AgentItem a)
        {
            return _getter(log, a);
        }

        public StatusMechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, StatusGetter getter) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _getter = getter;
            if (_getter == null)
            {
                throw new InvalidOperationException("Missing getter in StatusMechanic");
            }
        }
    }
}
