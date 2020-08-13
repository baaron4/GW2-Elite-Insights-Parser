using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class RevenantHelper : ProfHelper
    {
        internal static readonly List<InstantCastFinder> RevenantInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(26557, 27273, InstantCastFinder.DefaultICD), // Vengeful Hammers
        };
    }
}
