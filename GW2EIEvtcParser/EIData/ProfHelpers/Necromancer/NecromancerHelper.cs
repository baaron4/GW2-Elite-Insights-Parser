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
            new BuffGainCastFinder(10574, 790, 0), // Death shroud
            new BuffLossCastFinder(10585, 790, 0), // Death shroud
            new BuffGainCastFinder(10583, 10582, 1000), // Spectral Armor
            new BuffGainCastFinder(10685, 15083, 1000, 0, 94051), // Spectral Walk
            new BuffGainCastFinder(10685, 53476, 1000, 94051, ulong.MaxValue), // Spectral Walk
            //new BuffGainCastFinder(10635,???, 80647, ulong.MaxValue), // Lich's Gaze
        }; 
    }
}
