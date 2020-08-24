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
            new BuffGainCastFinder(28134, 27890, InstantCastFinder.DefaultICD), // Legendary Assassin Stance
            new BuffGainCastFinder(28494, 27928, InstantCastFinder.DefaultICD), // Legendary Demon Stance
            new BuffGainCastFinder(28419, 27205, InstantCastFinder.DefaultICD), // Legendary Dwarf Stance
            new BuffGainCastFinder(28195, 27972, InstantCastFinder.DefaultICD), // Legendary Centaur Stance
            new BuffGainCastFinder(27107, 27581, 500), // Impossible Odds
            new BuffLossCastFinder(28382, 27581, 500), // Relinquish Power
            new BuffGainCastFinder(26557, 27273, InstantCastFinder.DefaultICD), // Vengeful Hammers
            new BuffLossCastFinder(26956, 27273, InstantCastFinder.DefaultICD), // Release Hammers
        };

        private static readonly HashSet<long> _legendSwaps = new HashSet<long>
        {
            28134, 28494, 28419, 28195, 28085, 41858
        };

        public static bool IsLegendSwap(long id)
        {
            return _legendSwaps.Contains(id);
        }
    }
}
