using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class ReaperHelper : NecromancerHelper
    {
        internal static readonly List<InstantCastFinder> ReaperInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(30792, 29446, 0), // Reaper shroud
            new BuffLossCastFinder(30961, 29446, 0), // Reaper shroud
            new BuffGainCastFinder(29958, 30129, 0), // Infusing Terror
            new DamageCastFinder(29414, 29414, 500), // "You Are All Weaklings!"
            new DamageCastFinder(30670, 30670, 500), // "Suffer!"
            new DamageCastFinder(30772, 30772, 500), // "Rise!" --> better to check dark bond?
            new DamageCastFinder(29604, 29604, 500), // Chilling Nova
        };
    }
}
