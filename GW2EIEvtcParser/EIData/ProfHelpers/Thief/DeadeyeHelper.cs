using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    internal class DeadeyeHelper : ThiefHelper
    {

        internal static readonly List<InstantCastFinder> DeadeyeInstantCastFinders = new List<InstantCastFinder>()
        {
        };


        internal static readonly List<Buff> DeadeyeBuffs = new List<Buff>
        {
                new Buff("Kneeling",42869, ParserHelper.Source.Deadeye, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/56/Kneel.png"),
                new Buff("Deadeye's Gaze", 46333, ParserHelper.Source.Deadeye, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/78/Deadeye%27s_Mark.png"),
        };

    }
}
