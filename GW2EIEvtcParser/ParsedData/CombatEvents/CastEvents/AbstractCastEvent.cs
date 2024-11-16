using System.Runtime.CompilerServices;

namespace GW2EIEvtcParser.ParsedData;

public abstract class AbstractCastEvent : AbstractTimeCombatEvent
{

    public enum AnimationStatus { Unknown, Reduced, Interrupted, Full, Instant };

    // start item
    public SkillItem Skill { get; protected set; }
    public long SkillId => Skill.ID;
    public readonly AgentItem Caster;

    public AnimationStatus Status { get; protected set; } = AnimationStatus.Unknown;
    public int SavedDuration { get; protected set; }

    public int ExpectedDuration { get; protected set; }

    public int ActualDuration { get; protected set; }

    public long EndTime => Time + ActualDuration;
    public long ExpectedEndTime => Time + ExpectedDuration;

    public double Acceleration { get; protected set; } = 0;

    internal AbstractCastEvent(CombatItem baseItem, AgentData agentData, SkillData skillData) : base(baseItem.Time)
    {
        Skill = skillData.Get(baseItem.SkillID);
        Caster = agentData.GetAgent(baseItem.SrcAgent, baseItem.Time);
    }

    public AbstractCastEvent(long time, SkillItem skill, AgentItem caster) : base(time)
    {
        Skill = skill;
        Caster = caster;
    }

    public virtual long GetInterruptedByStunTime(ParsedEvtcLog log)
    {
        return EndTime;
    }
}

public static partial class ListExt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SortByTimeThenNegatedSwap<T>(this List<T> list)  where T : AbstractCastEvent
    {
        var str = string.Join(",", list.Select(x => x.Time));
        StableSort<T>.fluxsort(list.AsSpan(), (a, b) => a.Time.CompareTo(b.Time) * 2 + (Convert.ToInt32(b.Skill.IsSwap) - Convert.ToInt32(a.Skill.IsSwap)));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SortByTimeThenSwap<T>(this List<T> list)  where T : AbstractCastEvent
    {
        true.CompareTo(false);
        StableSort<T>.fluxsort(list.AsSpan(), (a, b) => a.Time.CompareTo(b.Time) * 2 + (Convert.ToInt32(a.Skill.IsSwap) - Convert.ToInt32(b.Skill.IsSwap)));
    }
}
