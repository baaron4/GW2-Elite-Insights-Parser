using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public abstract class CheckedMechanic<Checkable> : Mechanic
    {

        public delegate bool Checker(Checkable evt, ParsedEvtcLog log);
        private List<Checker> _checkers { get; set; }

        protected CheckedMechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _checkers = new List<Checker>();
        }

        internal CheckedMechanic<Checkable> UsingChecker(Checker checker)
        {
            _checkers.Add(checker);
            return this;
        }

        protected virtual bool Keep(Checkable checkable, ParsedEvtcLog log)
        {
            return _checkers.All(checker => checker(checkable, log));
        }

    }
}
