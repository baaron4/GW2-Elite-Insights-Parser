using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class EffectEvent : AbstractEffectEvent
    {

        /// <summary>
        /// Id of the created visual effect. Match to stable GUID with <see cref="EffectGUIDEvent"/>.
        /// </summary>
        public long EffectID { get; }

        /// <summary>
        /// End of the effect, provided by an <see cref="EffectEndEvent"/>
        /// </summary>
        internal long DynamicEndTime { get; private set; } = long.MinValue;

        internal bool HasDynamicEndTime => DynamicEndTime != long.MinValue;

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
        }

        internal void SetDynamicEndTime(EffectEndEvent endEvent)
        {
            // Security check
            if (TrackingID == 0)
            {
                return;
            }
            // We can only set the EndEventOnce
            if (!HasDynamicEndTime)
            {
                DynamicEndTime = endEvent.Time;
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
            if (HasDynamicEndTime)
            {
                return DynamicEndTime;
            }
            if (associatedBuff != null)
            {
                BuffRemoveAllEvent remove = log.CombatData.GetBuffDataByIDByDst(associatedBuff.Value, agent)
                    .OfType<BuffRemoveAllEvent>()
                    .FirstOrDefault(x => x.Time >= Time);
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
        /// Will use default duration if all other methods fail
        /// defaultDuration is ignored for <see cref="EffectEventCBTS45"/> and considered as 0.
        /// This method is to be used when the duration of the effect may not be static (ex: a trap AoE getting triggered or when a trait can modify the duration of a skill).
        /// See <see cref="ComputeEndTime"/> for information about computed end times.
        /// </summary>
        public virtual (long start, long end) ComputeDynamicLifespan(ParsedEvtcLog log, long defaultDuration, AgentItem agent = null, long? associatedBuff = null)
        {
            long durationToUse = defaultDuration;
            long start = Time;
            long end = ComputeEndTime(log, durationToUse, agent, associatedBuff);
            return (start, end);
        }

        /// <summary>
        /// Computes the lifespan of an effect.<br></br>
        /// Takes the <see cref="Time"/> of the main effect as start and the <see cref="Time"/> of the <paramref name="secondaryEffectGUID"/> as end.<br></br>
        /// Checks the matching effects Src.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="secondaryEffectGUID"><see cref="EffectGUIDs"/> of the secondary effect.</param>
        /// <returns>The computed start and end times.</returns>
        public (long start, long end) ComputeLifespanWithSecondaryEffect(ParsedEvtcLog log, string secondaryEffectGUID)
        {
            long start = Time;
            long end = start + Duration;
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(Src, secondaryEffectGUID, out IReadOnlyList<EffectEvent> effects))
            {
                EffectEvent firstEffect = effects.FirstOrDefault(x => x.Time >= Time && !IsAroundDst);
                if (firstEffect != null)
                {
                    end = firstEffect.Time;
                }
            }
            return (start, end);
        }

        /// <summary>
        /// Computes the lifespan of an effect.<br></br>
        /// Takes the <see cref="Time"/> of the main effect as start and the <see cref="Time"/> of the <paramref name="secondaryEffectGUID"/> as end.<br></br>
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="secondaryEffectGUID"><see cref="EffectGUIDs"/> of the secondary effect.</param>
        /// <returns>The computed start and end times.</returns>
        public (long start, long end) ComputeLifespanWithSecondaryEffectNoSrcCheck(ParsedEvtcLog log, string secondaryEffectGUID)
        {
            long start = Time;
            long end = start + Duration;
            if (log.CombatData.TryGetEffectEventsByGUID(secondaryEffectGUID, out IReadOnlyList<EffectEvent> effects))
            {
                EffectEvent firstEffect = effects.FirstOrDefault(x => x.Time >= Time && !IsAroundDst);
                if (firstEffect != null)
                {
                    end = firstEffect.Time;
                }
            }
            return (start, end);
        }

        /// <summary>
        /// Computes the lifespan of an effect.<br></br>
        /// Takes the <see cref="Time"/> of the main effect as start and the <see cref="Time"/> of the <paramref name="secondaryEffectGUID"/> as end.<br></br>
        /// Checks the matching effects Src and Position.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="secondaryEffectGUID"><see cref="EffectGUIDs"/> of the secondary effect.</param>
        /// <returns>The computed start and end times.</returns>
        public (long start, long end) ComputeLifespanWithSecondaryEffectAndPosition(ParsedEvtcLog log, string secondaryEffectGUID, double minDistance = 1e-6)
        {
            long start = Time;
            long end = start + Duration;
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(Src, secondaryEffectGUID, out IReadOnlyList<EffectEvent> effects))
            {
                EffectEvent firstEffect = effects.FirstOrDefault(x => x.Time >= Time && !x.IsAroundDst && x.Position.DistanceToPoint(Position) < minDistance);
                if (firstEffect != null)
                {
                    end = firstEffect.Time;
                }
            }
            return (start, end);
        }
    }
}
