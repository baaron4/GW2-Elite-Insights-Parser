using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public abstract class ExtensionMinionsHelper
    {

        internal Minions Minions { get; }

        internal IReadOnlyList<NPC> MinionList => Minions.MinionList;

        internal ExtensionMinionsHelper(Minions minions)
        {
            Minions = minions;
        }
    }
}
