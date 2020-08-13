using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class GuardianHelper : ProfHelper
    {
        internal static readonly List<InstantCastFinder> GuardianInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(9082, 9123, InstantCastFinder.DefaultICD), // Shield of Wrath
        };
    }
}
