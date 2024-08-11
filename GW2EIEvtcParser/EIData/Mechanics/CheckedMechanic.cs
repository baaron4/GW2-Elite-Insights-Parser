using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public abstract class CheckedMechanic<Checkable> : Mechanic
    {

        public delegate bool Checker(Checkable evt, ParsedEvtcLog log);
        protected List<Checker> Checkers { get; private set; }


        public delegate long TimeClamper(long time, ParsedEvtcLog log);
        private TimeClamper _timeClamper { get; set; }

        protected CheckedMechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            Checkers = new List<Checker>();
        }

        internal CheckedMechanic<Checkable> UsingChecker(Checker checker)
        {
            Checkers.Add(checker);
            return this;
        }

        internal CheckedMechanic<Checkable> UsingTimeClamper(TimeClamper clamper)
        {
            _timeClamper = clamper;
            return this;
        }

        protected void InsertMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, long time, AbstractSingleActor actor)
        {
            long timeToUse = time;
            if (_timeClamper != null)
            {
                timeToUse = _timeClamper(time, log);
            }
            mechanicLogs[this].Add(new MechanicEvent(timeToUse, this, actor));
        }

        protected virtual bool Keep(Checkable checkable, ParsedEvtcLog log)
        {
            return Checkers.All(checker => checker(checkable, log));
        }

    }
}
