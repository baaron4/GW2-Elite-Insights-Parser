using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class MesmerHelper : ProfHelper
    {
        internal static readonly List<InstantCastFinder> MesmerInstantCastFinders = new List<InstantCastFinder>()
        {
            new DamageCastFinder(10212, 10212, InstantCastFinder.DefaultICD), // Power spike
            new BuffLossCastFinder(10233, 10233, InstantCastFinder.DefaultICD, (brae, combatData) => {
                return combatData.GetBuffData(brae.To).Exists(x => 
                                    x is BuffApplyEvent bae &&
                                    bae.BuffID == 13017 &&
                                    Math.Abs(bae.AppliedDuration - 2000) <= ParserHelper.ServerDelayConstant &&
                                    bae.By == brae.To && 
                                    Math.Abs(brae.Time - bae.Time) <= ParserHelper.ServerDelayConstant
                                 );
                }
            ), // Signet of Midnight
        };
    }
}
