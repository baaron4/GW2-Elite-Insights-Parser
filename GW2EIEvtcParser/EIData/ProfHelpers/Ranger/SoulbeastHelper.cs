using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class SoulbeastHelper : RangerHelper
    {
        internal static readonly List<InstantCastFinder> SoulbeastInstantCastFinders = new List<InstantCastFinder>()
        {
            // Stout
            new BuffGainCastFinder(42944,40272,InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(43014,40272,InstantCastFinder.DefaultICD), // Leave Beastmode
            // Deadly
            new BuffGainCastFinder(42944,44932,InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(43014,44932,InstantCastFinder.DefaultICD), // Leave Beastmode
            // Versatile
            new BuffGainCastFinder(42944,44693,InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(43014,44693,InstantCastFinder.DefaultICD), // Leave Beastmode
            // Ferocious
            new BuffGainCastFinder(42944,41720,InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(43014,41720,InstantCastFinder.DefaultICD), // Leave Beastmode
            // Supportive
            new BuffGainCastFinder(42944,40069,InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(43014,40069,InstantCastFinder.DefaultICD), // Leave Beastmode
            // 
            new BuffGiveCastFinder(45789,41815,InstantCastFinder.DefaultICD), // Dolyak Stance
            new BuffGiveCastFinder(45970,45038,InstantCastFinder.DefaultICD), // Moa Stance
            new BuffGiveCastFinder(40498,44651,InstantCastFinder.DefaultICD), // Vulture Stance
        };
    }
}
