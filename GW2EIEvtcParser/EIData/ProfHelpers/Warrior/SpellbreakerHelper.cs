using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class SpellbreakerHelper : WarriorHelper
    {
        /////////////////////
        internal static readonly List<InstantCastFinder> SpellbreakerInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(43745, 40616, InstantCastFinder.DefaultICD), // Sight beyond Sight
            new DamageCastFinder(45534, 45534, InstantCastFinder.DefaultICD), // Loss Aversion

        };

    }
}
