using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class DeadeyeHelper : ThiefHelper
    {

        internal static readonly List<InstantCastFinder> DeadeyeInstantCastFinders = new List<InstantCastFinder>()
        {
            new DamageCastFinder(31600, 31600, InstantCastFinder.DefaultICD), // bound
        };
    }
}
