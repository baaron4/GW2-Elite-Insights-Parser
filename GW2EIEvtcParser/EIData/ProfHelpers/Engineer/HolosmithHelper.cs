using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class HolosmithHelper : EngineerHelper
    {
        internal static readonly List<InstantCastFinder> HolosmithInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(43937, 41037, InstantCastFinder.DefaultICD), // Overheat
            new BuffGainCastFinder(42938, 43708, InstantCastFinder.DefaultICD), // Photon Forge
            new BuffGainCastFinder(43066, 43708, InstantCastFinder.DefaultICD), // Spectrum Shield
        };
    }
}
