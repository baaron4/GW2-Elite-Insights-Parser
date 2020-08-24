using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class DruidHelper : RangerHelper
    {
        internal static readonly List<InstantCastFinder> DruidInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(31869,31508,InstantCastFinder.DefaultICD), // Celestial Avatar
            new BuffLossCastFinder(31411,31508,InstantCastFinder.DefaultICD), // Release Celestial Avatar
        };
    }
}
