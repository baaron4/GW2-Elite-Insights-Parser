using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public abstract class AnimatedCastEvent : CastEvent
{

    public readonly AnimationStart AnimStart;
    public readonly AnimationStop AnimStop;

    private void SetAcceleration(CombatItem endItem, int scaledActualDuration)
    {
        TimeScale = 1.0;
        if (scaledActualDuration > 0)
        {
            TimeScale = (double)scaledActualDuration / ActualDuration;
            if (TimeScale > 1.0)
            {
                // faster, convert to quickness percent
                AccelerationQuicknessSlow = (TimeScale - 1.0) / 0.5;
            }
            else
            {
                // slower, convert to slow percent
                AccelerationQuicknessSlow = -(1.0 - TimeScale) / 0.6;
            }
            AccelerationQuicknessSlow = Math.Max(Math.Min(AccelerationQuicknessSlow, 1.0), -1.0);
        }
        if (SkillID != SkillIDs.Resurrect)
        {
            switch (endItem.IsActivation)
            {
                case Activation.Cancel:
                    Status = AnimationStatus.Interrupted;
                    SavedDuration = -ActualDuration;
                    break;
                case Activation.Reset:
                    Status = AnimationStatus.Full;
                    break;
                case Activation.Minimum:
                case Activation.NoData:
                    SavedDuration = Math.Max(ExpectedScaledDuration - ActualDuration, 0);
                    Status = AnimationStatus.Reduced;
                    break;
            }
        }
        AccelerationQuicknessSlow = Math.Round(AccelerationQuicknessSlow, ParserHelper.AccelerationDigit);
    }
    protected AnimatedCastEvent(CombatItem? startItem, AgentData agentData, SkillData skillData, CombatItem? endItem, long maxEnd) : base(startItem ?? endItem ?? throw new InvalidOperationException("Either start or end item must be non null"), agentData, skillData)
    {
        var animWithScaledDurations = (startItem ?? endItem)?.Build >= ArcDPSBuilds.AnimWithScaledDurations;
        var animatedDodge = Skill.IsAnimatedDodge(skillData);
        // Start is present
        if (startItem != null)
        {
            if (animWithScaledDurations)
            {
                ExpectedDuration = startItem.BuffDmg;
                _expectedExecute = startItem.Value;
            }
            else
            {
                ExpectedDuration = startItem.Value;
            }
            if (startItem.IsStateChange == StateChange.AnimationStart)
            {
                if (startItem.DstAgent != 0)
                {
                    Target = agentData.GetAgent(startItem.DstAgent, startItem.Time);
                }
            }
            else
            {
                if (startItem.IsActivation == Activation.Quickness)
                {
                    AccelerationQuicknessSlow = 1;
                }
            }
            //_effectHappenedDuration = startItem.Value;

            // End item missing
            if (endItem == null)
            {
                if (animatedDodge)
                {
                    ExpectedDuration = 750;
                }
                ActualDuration = ExpectedDuration;
                CutAt(maxEnd);
            }
            AnimStart = GetAnimationStart(startItem.Result);
        }
        // End is present
        if (endItem != null)
        {
            ActualDuration = endItem.Value;
            var scaledActualDuration = animWithScaledDurations ? endItem.BuffDmg : 0;
            // Start missing
            if (startItem == null)
            {
                ExpectedDuration = ActualDuration;
                Time -= ActualDuration;
            }
            else
            {
                int expectedActualDuration = (int)(endItem.Time - startItem.Time);
                // Sanity check, sometimes the difference is massive
                if (Math.Abs(ActualDuration - expectedActualDuration) > ParserHelper.ServerDelayConstant)
                {
                    ActualDuration = expectedActualDuration;
                    scaledActualDuration = 0;
                }
            }
            if (animatedDodge)
            {
                // dodge animation start item has always 0 as expected duration
                ExpectedDuration = ActualDuration;
                scaledActualDuration = 0;
            }
            SetAcceleration(endItem, scaledActualDuration);
            AnimStop = GetAnimationStop(endItem.Result);

        }
    }

    internal void CutAt(long maxEnd)
    {
        if (EndTime > maxEnd && IsUnknown)
        {
            ActualDuration = (int)(maxEnd - Time);
        }
    }

    // Custom
    protected AnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AnimationStatus status = AnimationStatus.Full) : base(start, skill, caster)
    {
        if (status == AnimationStatus.Instant ||
            status == AnimationStatus.Reduced ||
            status == AnimationStatus.Interrupted)
        {
            throw new InvalidDataException("Custom Animated casts can't be instant, reduced or interrupted");
        }
        ActualDuration = (int)dur;
        ExpectedDuration = ActualDuration;
        AccelerationQuicknessSlow = 0;
        Status = status;
        SavedDuration = 0;
    }

    protected AnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AgentItem target, AnimationStatus status = AnimationStatus.Full) : this(caster, skill, start, dur, status)
    {
        Target = target;
    }

    public override long GetInterruptedByBuffTime(ParsedEvtcLog log, long buffID)
    {
        var buffStatus = log.FindActor(Caster).GetBuffStatus(log, buffID, Time, Math.Max(ExpectedEndTime, EndTime)).FirstOrNull((in x) => x.Value > 0);
        if (buffStatus != null)
        {
            return Math.Min(Math.Max(buffStatus.Value.Start, Time), EndTime);
        }
        return EndTime;
    }

    public override bool IgnoreOnRotationRender()
    {
        return false;
    }

    public bool IntersectsExpectedCastWindow(long time, long threshold = ParserHelper.ServerDelayConstant)
    {
        return time >= Time - threshold && ExpectedEndTime + threshold >= time;
    }
    public bool IntersectsActualCastWindow(long time, long threshold = ParserHelper.ServerDelayConstant)
    {
        return time >= Time - threshold && EndTime + threshold >= time;
    }
}
