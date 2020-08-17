using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class DragonhunterHelper : GuardianHelper
    {

        internal static readonly List<InstantCastFinder> DragonhunterInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffLossCastFinder(30039,29523,InstantCastFinder.DefaultICD), // Shield of Courage
        };
    }
}
