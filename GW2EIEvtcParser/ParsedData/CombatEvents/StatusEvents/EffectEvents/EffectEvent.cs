using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public abstract class EffectEvent : AbstractEffectEvent
{

    /// <summary>
    /// Id of the created visual effect. Match to stable GUID with <see cref="EffectGUIDEvent"/>.
    /// </summary>
    public readonly long EffectID;

    /// <summary>
    /// GUID event of the effect
    /// </summary>
    public GUID GUID { get; private set; } = EffectGUIDEvent.DummyEffectGUID.ContentGUID;

    /// <summary>
    /// End of the effect, provided by an <see cref="EffectEndEvent"/>
    /// </summary>
    internal long DynamicEndTime = long.MinValue;

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
    protected virtual long ComputeEndTime(ParsedEvtcLog log, long maxDuration, AgentItem? agent = null, long? associatedBuff = null)
    {
        if (HasDynamicEndTime)
        {
            return DynamicEndTime;
        }
        if (associatedBuff != null)
        {
            BuffRemoveAllEvent? remove = log.CombatData.GetBuffDataByIDByDst(associatedBuff.Value, agent!)
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
    public (long start, long end) ComputeLifespan(ParsedEvtcLog log, long defaultDuration, AgentItem? agent = null, long? associatedBuff = null)
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
    public virtual (long start, long end) ComputeDynamicLifespan(ParsedEvtcLog log, long defaultDuration, AgentItem? agent = null, long? associatedBuff = null)
    {
        long durationToUse = defaultDuration;
        long start = Time;
        long end = ComputeEndTime(log, durationToUse, agent, associatedBuff);
        return (start, end);
    }

    /// <summary>
    /// Computes the lifespan of an effect.<br></br>
    /// Takes the <see cref="Time"/> of the main effect as start and the <see cref="Time"/> of the <paramref name="secondaryEffect"/> as end.<br></br>
    /// Checks the matching effects Src.
    /// </summary>
    /// <param name="secondaryEffect"><see cref="EffectGUIDs"/> of the secondary effect.</param>
    /// <returns>The computed start and end times.</returns>
    public (long start, long end) ComputeLifespanWithSecondaryEffect(ParsedEvtcLog log, GUID secondaryEffect)
    {
        long start = Time;
        long end = start + Duration;
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(Src, secondaryEffect, out var effects))
        {
            EffectEvent? firstEffect = effects.FirstOrDefault(x => x.Time >= Time);
            if (firstEffect != null)
            {
                end = firstEffect.Time;
            }
        }
        return (start, end);
    }

    /// <summary>
    /// Computes the lifespan of an effect.<br></br>
    /// Takes the <see cref="Time"/> of the main effect as start and the <see cref="Time"/> of the <paramref name="secondaryEffect"/> as end.<br></br>
    /// </summary>
    /// <param name="secondaryEffect"><see cref="EffectGUIDs"/> of the secondary effect.</param>
    /// <returns>The computed start and end times.</returns>
    public (long start, long end) ComputeLifespanWithSecondaryEffectNoSrcCheck(ParsedEvtcLog log, GUID secondaryEffect)
    {
        long start = Time;
        long end = start + Duration;
        if (log.CombatData.TryGetEffectEventsByGUID(secondaryEffect, out var effects))
        {
            EffectEvent? firstEffect = effects.FirstOrDefault(x => x.Time >= Time);
            if (firstEffect != null)
            {
                end = firstEffect.Time;
            }
        }
        return (start, end);
    }

    /// <summary>
    /// Computes the lifespan of an effect.<br></br>
    /// Takes the <see cref="Time"/> of the main effect as start and the <see cref="Time"/> of the <paramref name="secondaryEffect"/> as end.<br></br>
    /// Checks the matching effects Src and Position.
    /// </summary>
    /// <returns>The computed start and end times.</returns>
    public (long start, long end) ComputeLifespanWithSecondaryEffectAndPosition(ParsedEvtcLog log, GUID secondaryEffect, double minDistance = 1e-6)
    {
        long start = Time;
        long end = start + Duration;
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(Src, secondaryEffect, out var effects))
        {
            EffectEvent? firstEffect = effects.FirstOrDefault(x => x.Time >= Time && !x.IsAroundDst && (x.Position - Position).Length() < minDistance);
            if (firstEffect != null)
            {
                end = firstEffect.Time;
            }
        }
        return (start, end);
    }


    /// <summary>
    /// Computes the lifespan of an effect.<br></br>
    /// Takes the <see cref="Math.Min(long, long)"/> between <see cref="DeadEvent"/>, <see cref="DespawnEvent"/> or Last Aware of a target as the End Time of the effect.
    /// </summary>
    /// <param name="targets">The list of targets to search the events of.</param>
    /// <param name="species">The type of species of the targets.</param>
    /// <returns>The computed start and end times.</returns>
    public (long start, long end) ComputeLifespanWithNPCRemoval(ParsedEvtcLog log, IReadOnlyList<SingleActor> targets, IReadOnlyList<int> species)
    {
        long start = Time;
        long end = start + Duration;

        foreach (SingleActor target in targets.Where(x => x.IsAnySpecies(species) && x.FirstAware <= start && x.LastAware >= start))
        {
            DeadEvent? deadEvent = log.CombatData.GetDeadEvents(target.AgentItem).FirstOrDefault();
            if (deadEvent != null)
            {
                end = Math.Min(deadEvent.Time, end);
            }
            DespawnEvent? despawnEvent = log.CombatData.GetDespawnEvents(target.AgentItem).FirstOrDefault();
            if (despawnEvent != null)
            {
                end = Math.Min(despawnEvent.Time, end);
            }
            end = Math.Min(target.LastAware, end);
        }
        return (start, end);
    }
    internal void SetGUIDEvent(CombatData combatData)
    {
        var guidEvent = combatData.GetEffectGUIDEvent(EffectID);
        GUID = guidEvent.ContentGUID;
        if (Duration == 0 && guidEvent.DefaultDuration > 0)
        {
            Duration = (long)Math.Min(guidEvent.DefaultDuration, int.MaxValue); // To avoid overflow, end time could be start + duration, 13 days is more than enough to cover a log's duration
        }
    }
}
