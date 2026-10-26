using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser.ParsedData;

public class SkillAnimation
{
    internal readonly SkillInfoEvent SkillInfo;

    public uint MinLoopSteps { get; protected set; } = 0;
    public uint MaxLoopSteps { get; protected set; } = 0;

    private readonly List<SkillTiming> _timings = [];
    private readonly List<SkillStep> _steps = [];
    public IReadOnlyList<SkillTiming> Timings => _timings;
    public IReadOnlyList<SkillStep> Steps => _steps;

    private Dictionary<SkillTiming, (SkillStep Step, int Delay)>? _timingToStep = null;
    private Dictionary<SkillStep, List<SkillTiming>>? _stepToTimings = null;

    internal int LoopStartIndex => _steps.FindIndex(step => step.IsLoopStart);
    internal int LoopEndIndex => _steps.FindIndex(step => step.IsLoopEnd);
    internal int LoopTermIndex => LoopEndIndex + 1;

    internal SkillAnimation(SkillInfoEvent skillInfo)
    {
        SkillInfo = skillInfo;
    }

    internal void SetLoopSteps(uint min, uint max)
    {
        MinLoopSteps = min;
        MaxLoopSteps = max;
    }

    internal void AddTiming(CombatItem evtcItem)
    {
        _timings.Add(SkillTiming.Build(evtcItem, this));
        DiscardTimingData();
    }

    internal void AddStep(CombatItem evtcItem)
    {
        _steps.Add(new SkillStep(evtcItem, this));
        DiscardTimingData();
    }

    public uint GetLoopSteps(bool minLoop)
    {
        return minLoop ? MinLoopSteps : MaxLoopSteps;
    }

    [MemberNotNull(nameof(_stepToTimings), nameof(_timingToStep))]
    private void InitTimingData()
    {
        if (_timingToStep == null || _stepToTimings == null)
        {
            _timingToStep = new(Timings.Count);
            _stepToTimings = new(Steps.Count);

            var stepTime = 0;
            var timingIndex = 0;
            foreach (var step in Steps)
            {
                var attached = new List<SkillTiming>();
                var stepEnd = stepTime + step.Duration;
                while (timingIndex < Timings.Count)
                {
                    var timing = Timings[timingIndex];
                    if (timing.Time >= stepEnd)
                    {
                        break;
                    }
                    var delay = timing.Time - stepTime;
                    _timingToStep.Add(timing, (step, delay));
                    attached.Add(timing);
                    timingIndex++;
                }
                _stepToTimings.Add(step, attached);
                stepTime = stepEnd;
            }
        }
    }

    private void DiscardTimingData()
    {
        _timingToStep = null;
        _stepToTimings = null;
    }

    public bool TryGetTriggerStep(SkillTiming timing, [NotNullWhen(true)] out SkillStep? step)
    {
        InitTimingData();
        if (_timingToStep.TryGetValue(timing, out var info))
        {
            step = info.Step;
            return true;
        }
        step = null;
        return false;
    }

    public bool TryGetTriggerDelay(SkillTiming timing, [NotNullWhen(true)] out int? delay)
    {
        InitTimingData();
        if (_timingToStep.TryGetValue(timing, out var info))
        {
            delay = info.Delay;
            return true;
        }
        delay = null;
        return false;
    }

    public IEnumerable<SkillStep> SimulateSteps(bool minLoop)
    {
        var loopSteps = GetLoopSteps(minLoop);
        var inLoop = false;
        var i = 0;
        while (i < Steps.Count)
        {
            var step = Steps[i];
            inLoop |= step.IsLoopStart;

            if (inLoop)
            {
                if (loopSteps == 0)
                {
                    inLoop = false;
                    i = LoopTermIndex; // jump to loop term
                    continue;
                }
                loopSteps--;
            }

            yield return step; // process step

            if (step.IsLoopEnd)
            {
                i = LoopStartIndex; // jump to loop start
                continue;
            }

            i++;
        }
    }

    public delegate bool StepPredicate(SkillStep step);
    public delegate bool TimingPredicate(SkillTiming timing);

    public int? ComputeFullStepDuration(TimeScale.Scaling? scaling = null, bool minLoop = false)
    {
        if (Steps.Count == 0)
        {
            return null;
        }
        return SimulateSteps(minLoop).Sum(step => TimeScale.ApplyScaling(step.Duration, scaling));
    }

    public int? ComputeStepTime(Func<SkillStep, bool> predicate, bool last, TimeScale.Scaling? scaling = null, bool minLoop = false)
    {
        var passed = 0;
        int? stepTime = null;
        foreach (var step in SimulateSteps(minLoop))
        {
            if (predicate(step))
            {
                stepTime = passed;
                if (!last)
                {
                    return stepTime;
                }
            }
            passed += TimeScale.ApplyScaling(step.Duration, scaling);
        }
        return stepTime;
    }

    public int? ComputeTriggerTime(Func<SkillTiming, bool> predicate, bool last, TimeScale.Scaling? scaling = null, bool minLoop = false)
    {
        InitTimingData();
        var passed = 0;
        int? triggerTime = null;
        foreach (var step in SimulateSteps(minLoop))
        {
            foreach (var timing in _stepToTimings[step])
            {
                if (predicate(timing))
                {
                    var delay = _timingToStep[timing].Delay;
                    triggerTime = passed + TimeScale.ApplyScaling(delay, scaling);
                    if (!last)
                    {
                        return triggerTime;
                    }
                }
            }
            passed += TimeScale.ApplyScaling(step.Duration, scaling);
        }
        return triggerTime;
    }

    public int? ComputeControlReturn(TimeScale.Scaling? scaling = null, bool minLoop = false)
    {
        return ComputeTriggerTime(timing => timing.IsControlReturn, false, scaling, minLoop) ?? ComputeFullStepDuration(scaling, minLoop);
    }

    public int? ComputeAnyExecute(bool last = false, TimeScale.Scaling? scaling = null, bool minLoop = false)
    {
        return ComputeTriggerTime(timing => timing.IsAnyExecute, last, scaling, minLoop);
    }
}
