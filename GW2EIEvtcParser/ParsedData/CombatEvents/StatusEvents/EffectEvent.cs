using System;
using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class EffectEvent : AbstractEffectEvent
    {

        /// <summary>
        /// Id of the created visual effect. Match to stable GUID with <see cref="EffectGUIDEvent"/>.
        /// </summary>
        public long EffectID { get; }
        /// <summary>
        /// End of the effect
        /// </summary>
        public EffectEndEvent EndEvent { get; private set; }

        /// <summary>
        /// Duration of the effect in milliseconds.
        /// </summary>
        public long Duration { get; protected set; }

        /// <summary>
        /// If true, effect is on a moving platform
        /// </summary>
        public bool OnNonStaticPlatform { get; protected set; }

        internal EffectEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            EffectID = evtcItem.SkillID;
            EndEvent = null;
        }

        internal void SetEndEvent(EffectEndEvent endEvent)
        {
            // Security check
            if (TrackingID == 0)
            {
                return;
            }
            // We can only set the EndEventOnce
            if (EndEvent == null)
            {
                EndEvent = endEvent;
            }
        }
    }
}
