using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class CastMechanic : IDBasedMechanic
    {
        public delegate bool CastChecker(AbstractCastEvent ce, ParsedEvtcLog log);

        private readonly CastChecker _triggerCondition = null;

        protected bool Keep(AbstractCastEvent c, ParsedEvtcLog log)
        {
            if (_triggerCondition != null)
            {
                return _triggerCondition(c, log);
            }
            return true;
        }

        protected virtual long GetTime(AbstractCastEvent evt)
        {
            return evt.Time;
        }

        public CastMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CastChecker condition = null) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
        }
    }
}
