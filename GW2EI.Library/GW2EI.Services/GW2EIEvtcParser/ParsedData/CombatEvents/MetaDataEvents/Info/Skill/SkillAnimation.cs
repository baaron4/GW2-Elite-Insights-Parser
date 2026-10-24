using System.Diagnostics.CodeAnalysis;

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
    }

    internal void AddStep(CombatItem evtcItem)
    {
        _steps.Add(new SkillStep(evtcItem, this));
    }

    public uint GetLoopSteps(bool minLoop)
    {
        return minLoop ? MinLoopSteps : MaxLoopSteps;
    }

    public bool TryFindTriggerStep(int triggerTime, [NotNullWhen(true)] out SkillStep? triggerStep, [NotNullWhen(true)] out int? triggerDelay)
    {
        int stepTime = 0;
        foreach (var step in Steps)
        {
            var stepEnd = stepTime + step.Duration;
            if (triggerTime < stepEnd)
            {
                triggerStep = step;
                triggerDelay = triggerTime - stepTime;
                return true;
            }
            stepTime = stepEnd;
        }
        triggerStep = null;
        triggerDelay = null;
        return false;
    }

    public bool TryFindTriggerStep(SkillTiming timing, [NotNullWhen(true)] out SkillStep? triggerStep, [NotNullWhen(true)] out int? triggerDelay)
    {
        return TryFindTriggerStep(timing.Time, out triggerStep, out triggerDelay);
    }

    public IEnumerable<SkillStep> Simulate(bool minLoop)
    {
        var loopSteps = GetLoopSteps(minLoop);
        bool inLoop = false;
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

    public int? ComputeFullStepDuration(bool minLoop = false)
    {
        if (Steps.Count == 0)
        {
            return null;
        }
        return Simulate(minLoop).Sum(step => step.Duration);
    }

    public int? ComputeControlReturn(bool minLoop = false)
    {
        var controlReturn = Timings.FirstOrDefault(timing => timing.IsControlReturn);
        if (controlReturn != null && TryFindTriggerStep(controlReturn, out var triggerStep, out var triggerDelay))
        {
            var passed = 0;
            foreach (var step in Simulate(minLoop))
            {
                if (step == triggerStep)
                {
                    return passed + triggerDelay.Value;
                }
                passed += step.Duration;
            }
            return passed;
        }
        else
        {
            return ComputeFullStepDuration(minLoop);
        }
    }
}
