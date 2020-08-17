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
            new BuffGainCastFinder(9104, 9103, 0), // Zealot's Flame
            new BuffLossCastFinder(9115,9114,InstantCastFinder.DefaultICD), // Virtue of Justice
            new BuffLossCastFinder(9120,9119,InstantCastFinder.DefaultICD), // Virtue of Resolve
            new BuffLossCastFinder(9118,9113,InstantCastFinder.DefaultICD), // Virtue of Courage
            //new BuffLossCastFinder(9242,9156,InstantCastFinder.DefaultICD), // Signet of Judgment
            //new BuffLossCastFinder(9242,9239,InstantCastFinder.DefaultICD), // Signet of Judgment PI
            new DamageCastFinder(9247,9247, InstantCastFinder.DefaultICD), // Judge's Intervention
        };
    }
}
