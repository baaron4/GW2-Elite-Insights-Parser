using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class FirebrandHelper : GuardianHelper
    {
        internal static readonly List<InstantCastFinder> FirebrandInstantCastFinders = new List<InstantCastFinder>()
        {
            new DamageCastFinder(46618,46618,InstantCastFinder.DefaultICD), // Flame Rush
            new DamageCastFinder(46616,46616,InstantCastFinder.DefaultICD), // Flame Surge
            //new DamageCastFinder(42360,42360,InstantCastFinder.DefaultICD), // Echo of Truth
            //new DamageCastFinder(44008,44008,InstantCastFinder.DefaultICD), // Voice of Truth
        };
    }
}
