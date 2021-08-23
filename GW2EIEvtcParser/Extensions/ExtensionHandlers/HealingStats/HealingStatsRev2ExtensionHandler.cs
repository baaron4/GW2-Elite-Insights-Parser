using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    internal class HealingStatsRev2ExtensionHandler : HealingStatsRev1ExtensionHandler
    {

        internal HealingStatsRev2ExtensionHandler(CombatItem c) : base(c)
        {
            Revision = 2;
        }

    }
}
