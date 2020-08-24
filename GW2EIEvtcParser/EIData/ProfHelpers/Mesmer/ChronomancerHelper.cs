using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    internal class ChronomancerHelper : MesmerHelper
    {
        internal static readonly List<InstantCastFinder> ChronomancerInstantCastFinders = new List<InstantCastFinder>()
        {
        };


        internal static readonly List<Buff> ChronomancerBuffs = new List<Buff>
        {
                new Buff("Time Echo",29582, ParserHelper.Source.Chronomancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8d/Deja_Vu.png"),
                new Buff("Time Anchored",30136, ParserHelper.Source.Chronomancer, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/db/Continuum_Split.png"),
        };

    }
}
