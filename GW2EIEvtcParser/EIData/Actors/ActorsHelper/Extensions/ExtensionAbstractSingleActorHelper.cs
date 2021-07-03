using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public abstract class ExtensionAbstractSingleActorHelper
    {

        internal AbstractSingleActor Actor { get; }

        internal AgentItem AgentItem => Actor.AgentItem;

        internal ExtensionAbstractSingleActorHelper(AbstractSingleActor actor)
        {
            Actor = actor;
        }
    }
}
