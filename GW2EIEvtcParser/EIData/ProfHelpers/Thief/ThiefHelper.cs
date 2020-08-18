using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class ThiefHelper : ProfHelper
    {
        internal static readonly List<InstantCastFinder> ThiefInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(13002,13135,InstantCastFinder.DefaultICD), // Shadowstep
            new BuffLossCastFinder(13106,13135,InstantCastFinder.DefaultICD, (evt, combatData) => evt.RemovedDuration > ParserHelper.ServerDelayConstant), // Shadow Return
            new BuffGainCastFinder(13046,44597,InstantCastFinder.DefaultICD), // Assassin's Signet
            new BuffGiveCastFinder(13093,13094,InstantCastFinder.DefaultICD), // Devourer Venom
            new BuffGiveCastFinder(13096,13095,InstantCastFinder.DefaultICD), // Ice Drake Venom
            new BuffGiveCastFinder(13055,13054,InstantCastFinder.DefaultICD), // Skale Venom
            //new BuffGiveCastFinder(13037,13036,InstantCastFinder.DefaultICD), // Spider Venom - same id as leeching venom trait?
        };
    }
}
