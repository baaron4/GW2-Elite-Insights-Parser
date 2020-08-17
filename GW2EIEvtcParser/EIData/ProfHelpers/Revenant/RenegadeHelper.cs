using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class RenegadeHelper : RevenantHelper
    {

        internal static readonly List<InstantCastFinder> RenegadeInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(41858, 44272, InstantCastFinder.DefaultICD), // Legendary Renegade Stance
        };
    }
}
