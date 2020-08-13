using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class NecromancerHelper : ProfHelper
    {
        internal static readonly List<InstantCastFinder> NecromancerInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(10574, 790, InstantCastFinder.DefaultICD), // Death shroud
            new BuffLossCastFinder(10585, 790, InstantCastFinder.DefaultICD), // Death shroud
            new BuffGainCastFinder(10583, 10582, InstantCastFinder.DefaultICD), // Spectral Armor
            new BuffGainCastFinder(10685, 15083, InstantCastFinder.DefaultICD, 0, 94051), // Spectral Walk
            new BuffGainCastFinder(10685, 53476, InstantCastFinder.DefaultICD, 94051, ulong.MaxValue), // Spectral Walk
            //new BuffGainCastFinder(10635,???, 80647, ulong.MaxValue), // Lich's Gaze
        };

        private static readonly HashSet<long> _shroudTransform = new HashSet<long>
        {
            10574,10585,30792, 30961,
        };

        public static bool IsShroudTransform(long id)
        {
            return _shroudTransform.Contains(id);
        }
    }
}
