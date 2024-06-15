using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public abstract class CheckedMechanic<Checkable> : Mechanic
    {

        public delegate bool Checker(Checkable evt, ParsedEvtcLog log);
        protected List<Checker> Checkers { get; private set; }

        protected CheckedMechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            Checkers = new List<Checker>();
        }

        internal CheckedMechanic<Checkable> UsingChecker(Checker checker)
        {
            Checkers.Add(checker);
            return this;
        }

        protected virtual bool Keep(Checkable checkable, ParsedEvtcLog log)
        {
            return Checkers.All(checker => checker(checkable, log));
        }

    }
}
