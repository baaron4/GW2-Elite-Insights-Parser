using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class HeraldHelper : RevenantHelper
    {
        internal static readonly List<InstantCastFinder> HeraldInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(28085, 27732, InstantCastFinder.DefaultICD), // Legendary Dragon Stance
            new BuffGainCastFinder(29371, 29275, InstantCastFinder.DefaultICD), // Facet of Nature
            new BuffGainCastFinder(28379, 28036, InstantCastFinder.DefaultICD), // Facet of Darkness
            new BuffGainCastFinder(27014, 28243, InstantCastFinder.DefaultICD), // Facet of Elements
            new BuffGainCastFinder(26644, 27376, InstantCastFinder.DefaultICD), // Facet of Strength
            new BuffGainCastFinder(27760, 27983, InstantCastFinder.DefaultICD), // Facet of Chaos
        };
    }
}
