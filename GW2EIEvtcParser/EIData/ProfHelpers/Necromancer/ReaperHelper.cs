using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    internal class ReaperHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(30792, 29446, EIData.InstantCastFinder.DefaultICD), // Reaper shroud
            new BuffLossCastFinder(30961, 29446, EIData.InstantCastFinder.DefaultICD), // Reaper shroud
            new BuffGainCastFinder(29958, 30129, EIData.InstantCastFinder.DefaultICD), // Infusing Terror
            new DamageCastFinder(29414, 29414, EIData.InstantCastFinder.DefaultICD), // "You Are All Weaklings!"
            new DamageCastFinder(30670, 30670, EIData.InstantCastFinder.DefaultICD), // "Suffer!"
            new DamageCastFinder(30772, 30772, EIData.InstantCastFinder.DefaultICD), // "Rise!" --> better to check dark bond?
            new DamageCastFinder(29604, 29604, EIData.InstantCastFinder.DefaultICD), // Chilling Nova
        };
        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Reaper's Shroud", 29446, ParserHelper.Source.Reaper, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/11/Reaper%27s_Shroud.png"),
        };
    }
}
