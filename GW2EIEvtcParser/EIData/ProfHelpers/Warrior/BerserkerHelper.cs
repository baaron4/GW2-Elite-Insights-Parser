using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class BerserkerHelper : WarriorHelper
    {
        /////////////////////
        internal static readonly List<InstantCastFinder> BerserkerInstantCastFinders = new List<InstantCastFinder>()
        {
            new DamageCastFinder(31289, 31289, 500, 97950, ulong.MaxValue), // King of Fires
        };

    }
}
