using System.Numerics;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public abstract class PhaseData
{
    public long Start { get; private set; }
    public long End { get; private set; }
    public string DurationString
    {
        get
        {
            return ParserHelper.ToDurationString(End - Start);
        }
    }
    public long DurationInS { get; private set; }
    public long DurationInMS { get; private set; }
    public long DurationInM { get; private set; }
    public string Name { get; internal set; } = "";
    public bool DrawStart { get; internal set; } = true;
    public bool DrawEnd { get; internal set; } = true;
    public bool DrawArea { get; internal set; } = true;
    public bool DrawLabel { get; internal set; } = true;
    private readonly HashSet<PhaseData> CanBeSubPhaseOf = [];

    public bool BreakbarPhase { get; protected set; } = false;
    
    public PhaseType Type { get; protected set; }
    public enum PhaseType
    {
        SubPhase = 0,
        Encounter = 1,
        Instance = 2,
        TimeFrame = 3,
    };

    public enum TargetPriority
    {
        Main = 0,
        Blocking = 1,
        NonBlocking = 2,
    }

    public class PhaseTargetData
    {
        public TargetPriority Priority { get; internal set; }

        public bool IsPrioritary(TargetPriority priority)
        {
            return (int)Priority <= (int)priority;
        }
    }

    public IReadOnlyDictionary<SingleActor, PhaseTargetData> Targets => _targets;
    private readonly Dictionary<SingleActor, PhaseTargetData> _targets = [];

    internal PhaseData(long start, long end) : this(start, end, PhaseType.TimeFrame)
    {
    }

    protected PhaseData(long start, long end, PhaseType type)
    {
        Start = start;
        End = end;
        DurationInM = (End - Start) / 60000;
        DurationInMS = (End - Start);
        DurationInS = (End - Start) / 1000;
        Type = type;
    }

    internal PhaseData(long start, long end, string name) : this(start, end)
    {
        Name = name;
    }

    protected PhaseData(long start, long end, string name, PhaseType type) : this(start, end, type)
    {
        Name = name;
    }

    public bool InInterval(long time)
    {
        return Start <= time && time <= End;
    }

    public bool IntersectsWindow(long start, long end)
    {
        long maxStart = Math.Max(start, Start);
        long minEnd = Math.Min(end, End);
        return minEnd - maxStart > 0;
    }

    internal void RemoveTarget(SingleActor target)
    {
        //TODO_PERF(Rennorb)
        _targets.Remove(target);
    }

    internal void AddTarget(SingleActor? target, ParsedEvtcLog log, TargetPriority priority = TargetPriority.Main)
    {
        if (target == null)
        {
            return;
        }
        var (_, _, _, actives) = target.GetStatus(log);
        if (!actives.Any(x => IntersectsWindow(x.Start, x.End)))
        {
            return;
        }
        if (_targets.TryGetValue(target, out var targetData))
        {
            if (!targetData.IsPrioritary(priority))
            {
                targetData.Priority = priority;
            }
        } else
        {
            _targets[target] = new PhaseTargetData
            {
                Priority = priority,
            };
        }
    }

    internal void AddTargets(IEnumerable<SingleActor?> targets, ParsedEvtcLog log, TargetPriority priority = TargetPriority.Main)
    {
        foreach (SingleActor? target in targets)
        {
            AddTarget(target, log, priority);
        }
    }

    internal void OverrideStart(long start)
    {
        Start = start;
        DurationInM = (End - Start) / 60000;
        DurationInMS = (End - Start);
        DurationInS = (End - Start) / 1000;
    }

    internal void OverrideEnd(long end)
    {
        End = end;
        DurationInM = (End - Start) / 60000;
        DurationInMS = (End - Start);
        DurationInS = (End - Start) / 1000;
    }

    /// <summary>
    /// Override times in a manner that the phase englobes the targets activities in the phase
    /// </summary>
    /// <param name="log"></param>
    internal void OverrideTimes(ParsedEvtcLog log)
    {
        OverrideStartTime(log);
        OverrideEndTime(log);
    }
    /// <summary>
    /// Override start in a manner that the phase starts when the targets are active
    /// </summary>
    /// <param name="log"></param>
    internal void OverrideStartTime(ParsedEvtcLog log)
    {
        if (Targets.Count > 0)
        {
            long start = long.MaxValue;
            foreach (var pair in Targets)
            {
                var target = pair.Key;
                long startTime = target.FirstAware;
                EnterCombatEvent? enterCombat = log.CombatData.GetEnterCombatEvents(target.AgentItem).FirstOrDefault();
                if (enterCombat != null)
                {
                    startTime = enterCombat.Time;
                } 
                else
                {
                    SpawnEvent? spawned = log.CombatData.GetSpawnEvents(target.AgentItem).FirstOrDefault();
                    if (spawned != null)
                    {
                        startTime = spawned.Time;
                    }
                }
                start = Math.Min(start, startTime);
            }
            OverrideStart(Math.Max(Math.Max(Start, start), log.LogData.LogStart));
        }
    }
    /// <summary>
    /// Override end in a manner that the phase ends when the targets are gone (if possible)
    /// </summary>
    /// <param name="log"></param>
    internal void OverrideEndTime(ParsedEvtcLog log)
    {
        if (Targets.Count > 0)
        {
            long end = long.MinValue;
            foreach (var pair in Targets)
            {
                var target = pair.Key;
                long endTime = target.LastAware;
                var died = log.CombatData.GetDeadEvents(target.AgentItem).FirstOrDefault();
                if (died != null)
                {
                    endTime = died.Time;
                } 
                else
                {
                    var despawned = log.CombatData.GetDespawnEvents(target.AgentItem).LastOrDefault();
                    if (despawned != null)
                    {
                        endTime = despawned.Time;
                    }
                }
                end = Math.Max(end, endTime);
            }
            OverrideEnd(Math.Min(Math.Min(End, end), log.LogData.LogEnd));
        }
    }

    internal PhaseData WithParentPhase(PhaseData? phase)
    {
        AddParentPhase(phase);
        return this;
    }

    internal virtual void AddParentPhase(PhaseData? phase)
    {
        if (phase != null)
        {
            CanBeSubPhaseOf.Add(phase);
        }
    }

    internal PhaseData WithParentPhases(IEnumerable<PhaseData?> phases)
    {
        AddParentPhases(phases);
        return this;
    }

    internal void AddParentPhases(IEnumerable<PhaseData?> phases)
    {
        foreach (var phase in phases)
        {
            AddParentPhase(phase);
        }
    }

    public bool CanBeASubPhaseOf(PhaseData phase)
    {
        return CanBeSubPhaseOf.Contains(phase);
    }
}
