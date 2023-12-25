using System;
using System.Collections.Generic;
using System.Linq;
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
        private EffectEndEvent EndEvent { get; set; }

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

        /// <summary>
        /// Computes the end time of an effect.
        /// <br/>
        /// When no end event is present, it falls back to buff remove all of associated buff (if passed) first.
        /// Afterwards the effect duration is used, if greater 0 and less than max duration.
        /// Finally, it defaults to max duration.
        /// </summary>
        protected virtual long ComputeEndTime(ParsedEvtcLog log, long maxDuration, AgentItem agent = null, long? associatedBuff = null)
        {
            if (EndEvent != null)
            {
                return EndEvent.Time;
            }
            if (associatedBuff != null)
            {
                BuffRemoveAllEvent remove = log.CombatData.GetBuffData(associatedBuff.Value)
                    .OfType<BuffRemoveAllEvent>()
                    .FirstOrDefault(x => x.To == agent && x.Time >= Time);
                if (remove != null)
                {
                    return remove.Time;
                }
            }
            if (Duration > 0 && Duration <= maxDuration)
            {
                return Time + Duration;
            }
            return Time + maxDuration;
        }


        /// <summary>
        /// Computes the lifespan of an effect.
        /// Will use default duration if all other methods fail
        /// See <see cref="ComputeEndTime"/> for information about computed end times.
        /// </summary>
        public (long start, long end) ComputeLifespan(ParsedEvtcLog log, long defaultDuration, AgentItem agent = null, long? associatedBuff = null)
        {
            long start = Time;
            long end = ComputeEndTime(log, defaultDuration, agent, associatedBuff);
            return (start, end);
        }

        /// <summary>
        /// Computes the lifespan of an effect.
        /// Will default to 0 duration if all other methods fail.
        /// This method is to be used when the duration of the effect is not static (ex: a trap AoE getting triggered or when a trait can modify the duration).
        /// See <see cref="ComputeEndTime"/> for information about computed end times.
        /// </summary>
        public virtual (long start, long end) ComputeDynamicLifespan(ParsedEvtcLog log, long defaultDuration, AgentItem agent = null, long? associatedBuff = null)
        {
            long durationToUse = defaultDuration;
            long start = Time;
            long end = ComputeEndTime(log, durationToUse, agent, associatedBuff);
            return (start, end);
        }
    }
}
