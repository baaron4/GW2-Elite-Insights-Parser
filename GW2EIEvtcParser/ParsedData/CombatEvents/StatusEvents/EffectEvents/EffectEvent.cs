using System.Numerics;
using static GW2EIEvtcParser.EIData.Trigonometry;
using static GW2EIEvtcParser.ParserHelper;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public abstract class EffectEvent : StatusEvent
{

    /// <summary>
    /// The effect's rotation around each axis in <b>radians</b>.
    /// Use <see cref="Rotation"/> for degrees.
    /// </summary>
    public Vector3 Orientation { get; protected set; }

    /// <summary>
    /// The effect's rotation around each axis in <b>degrees</b>.
    /// Like <see cref="Orientation"/> but using degrees.
    /// </summary>
    public Vector3 Rotation => new(RadianToDegreeF(Orientation.X), RadianToDegreeF(Orientation.Y), RadianToDegreeF(Orientation.Z));

    /// <summary>
    /// The effect's position in the game's coordinate system, if <see cref="IsAroundDst"/> is <c>false</c>.
    /// </summary>
    public Vector3 Position { get; protected set; } = new(0, 0, 0);

    /// <summary>
    /// Whether the effect location is following <see cref="Dst"/> or located at <see cref="Position"/>.
    /// </summary>
    public bool IsAroundDst => _dst != null;
    /// <summary>
    /// The agent the effect is located at, if <see cref="IsAroundDst"/> is <c>true</c>.
    /// </summary>
    protected AgentItem? _dst { get; set; } = null;
    /// <summary>
    /// The agent the effect is located at, if <see cref="IsAroundDst"/> is <c>true</c>.
    /// </summary>
    public AgentItem Dst => _dst ?? _unknownAgent;

    /// <summary>
    /// Unique id for tracking a created effect.
    /// </summary>
    protected long TrackingID;
    /// <summary>
    /// Id of the created visual effect. Match to stable GUID with <see cref="EffectGUIDEvent"/>.
    /// </summary>
    public readonly long EffectID;

    /// <summary>
    /// GUID event of the effect
    /// </summary>
    public readonly EffectGUIDEvent GUIDEvent = EffectGUIDEvent.DummyEffectGUID;

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
    /// <summary>
    /// Scale of the effect
    /// </summary>
    public float Scale { get; protected set; } = -1.0f;

    public bool IsScaled => Scale >= 0.0f;

    /// <summary>
    /// Scale something of the effect
    /// </summary>
    public float ScaleSomething { get; protected set; } = 1.0f;
    /// <summary>
    /// Flags of the effect
    /// </summary>
    public byte Flags { get; protected set; } = 0;

    internal EffectEvent(CombatItem evtcItem, AgentData agentData, IReadOnlyDictionary<long, EffectGUIDEvent> effectGUIDs) : base(evtcItem, agentData)
    {
        EffectID = evtcItem.SkillID;
        if (effectGUIDs.TryGetValue(evtcItem.SkillID, out var effectGUID))
        {
            GUIDEvent = effectGUID;
        }
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
            if (maxDuration > 0)
            {
                return Math.Min(DynamicEndTime, Time + maxDuration);
            }
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
    /// Takes the <see cref="Time"/> of the main effect as start and the <see cref="Time"/> of the first effect in <paramref name="secondaryEffects"/> found as end.<br></br>
    /// Checks the matching effects Src.
    /// </summary>
    /// <param name="secondaryEffects"><see cref="EffectGUIDs"/> of the secondary effects.</param>
    /// <returns>The computed start and end times.</returns>
    public (long start, long end) ComputeLifespanWithSecondaryEffects(ParsedEvtcLog log, GUID[] secondaryEffects)
    {
        long start = Time;
        long end = start + Duration;
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(Src, secondaryEffects, out var effects))
        {
            effects.SortByTime();
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

    public bool IsAtHeight(float height, float epsilon = 10f)
    {
        return Math.Abs(Position.Z - height) < epsilon;
    }

    public bool IsAboveHeight(float height, float epsilon = 10f)
    {
        return Position.Z < height + epsilon; // towards negative = higher
    }

    public bool IsBelowHeight(float height, float epsilon = 10f)
    {
        return Position.Z > height - epsilon; // towards positive = lower
    }
}
