using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class DaredevilHelper : ThiefHelper
    {
        internal static readonly List<InstantCastFinder> DaredevilInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(31600, 33162, InstantCastFinder.DefaultICD), // bound
        };
    }
}
