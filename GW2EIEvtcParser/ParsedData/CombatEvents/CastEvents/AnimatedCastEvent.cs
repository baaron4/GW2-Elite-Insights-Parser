using System.Numerics;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class AnimatedCastEvent : CastEvent
{
    private readonly int _scaledActualDuration;
    //private readonly int _effectHappenedDuration;
    public AgentItem EffectTarget { get; protected set; } = ParserHelper._unknownAgent;

    public readonly AnimationStart AnimStart;
    public readonly AnimationStop AnimStop;

    private void SetAcceleration(CombatItem endItem)
    {
        double nonScaledToScaledRatio = 1.0;
        if (_scaledActualDuration > 0)
        {
            nonScaledToScaledRatio = (double)_scaledActualDuration / ActualDuration;
            if (nonScaledToScaledRatio > 1.0)
            {
                // faster
                Acceleration = (nonScaledToScaledRatio - 1.0) / 0.5;
            }
            else
            {
                Acceleration = -(1.0 - nonScaledToScaledRatio) / 0.6;
            }
            Acceleration = Math.Max(Math.Min(Acceleration, 1.0), -1.0);
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
                    int scaledExpectedDuration = (int)Math.Round(ExpectedDuration / nonScaledToScaledRatio);
                    SavedDuration = Math.Max(scaledExpectedDuration - ActualDuration, 0);
                    Status = AnimationStatus.Reduced;
                    break;
            }
        }
        AcceleratedToNonAcceleratedRatio = 1.0 / nonScaledToScaledRatio;
        Acceleration = Math.Round(Acceleration, ParserHelper.AccelerationDigit);
    }

    protected const float PositionConvertConstant = 10.0f;
    internal AnimatedCastEvent(CombatItem? startItem, AgentData agentData, SkillData skillData, CombatItem? endItem, long maxEnd) : base(startItem ?? endItem ?? throw new InvalidOperationException("Either start or end item must be non null"), agentData, skillData)
    {
        // Start is present
        if (startItem != null)
        {
            ExpectedDuration = startItem.BuffDmg > 0 ? startItem.BuffDmg : startItem.Value;
            if (startItem.IsStateChange == StateChange.AnimationStart)
            {

                if (startItem.DstAgent != 0)
                {
                    EffectTarget = agentData.GetAgent(startItem.DstAgent, startItem.Time);
                }
            } 
            else
            {
                if (startItem.IsActivation == Activation.Quickness)
                {
                    Acceleration = 1;
                }
            }
            //_effectHappenedDuration = startItem.Value;

            // End item missing
            if (endItem == null)
            {
                if (Skill.ID == skillData.DodgeID)
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
            _scaledActualDuration = endItem.BuffDmg;
            // Start missing
            if (startItem == null)
            {
                ExpectedDuration = ActualDuration;
                if (Skill.IsAnimatedDodge(skillData))
                {
                    // dodge animation start item has always 0 as expected duration
                    ExpectedDuration = ActualDuration;
                    _scaledActualDuration = 0;
                }
                Time -= ActualDuration;
                SetAcceleration(endItem);
            } 
            else
            {
                int expectedActualDuration = (int)(endItem.Time - startItem.Time);
                // Sanity check, sometimes the difference is massive
                if (Math.Abs(ActualDuration - expectedActualDuration) > ParserHelper.ServerDelayConstant)
                {
                    ActualDuration = expectedActualDuration;
                    _scaledActualDuration = 0;
                }
                if (Skill.IsAnimatedDodge(skillData))
                {
                    // dodge animation start item has always 0 as expected duration
                    ExpectedDuration = ActualDuration;
                    _scaledActualDuration = 0;
                }
                SetAcceleration(endItem);
            }
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
    internal AnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur) : base(start, skill, caster)
    {
        ActualDuration = (int)dur;
        ExpectedDuration = ActualDuration;
        Acceleration = 0;
        Status = AnimationStatus.Full;
        SavedDuration = 0;
    }

    internal AnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AgentItem effectTarget) : this(caster, skill, start, dur)
    {
        EffectTarget = effectTarget;
    }

    public override long GetInterruptedByBuffTime(ParsedEvtcLog log, long buffID)
    {
        var buffStatus = log.FindActor(Caster).GetBuffStatus(log, buffID, Time, ExpectedEndTime).FirstOrNull((in Segment x) => x.Value > 0);
        if (buffStatus != null)
        {
            return Math.Max(buffStatus.Value.Start, Time);
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
