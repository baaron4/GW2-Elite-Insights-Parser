using System;

namespace GW2EIEvtcParser.ParsedData
{
    public class AnimatedCastEvent : AbstractCastEvent
    {
        private readonly int _scaledActualDuration;
        //private readonly int _effectHappenedDuration;

        private AnimatedCastEvent(CombatItem startItem, AgentData agentData, SkillData skillData) : base(startItem, agentData, skillData)
        {
            ExpectedDuration = startItem.BuffDmg > 0 ? startItem.BuffDmg : startItem.Value;
            if (startItem.IsActivation == ArcDPSEnums.Activation.Quickness)
            {
                Acceleration = 1;
            }
            //_effectHappenedDuration = startItem.Value;
        }

        private void SetAcceleration(CombatItem endItem)
        {
            double nonScaledToScaledRatio = 1.0;
            if (_scaledActualDuration > 0)
            {
                nonScaledToScaledRatio = (double)_scaledActualDuration / ActualDuration;
                if (nonScaledToScaledRatio > 1.0)
                {
                    // faster
                    Acceleration = (nonScaledToScaledRatio - 1.0) / 0.66;
                }
                else
                {
                    Acceleration = -(1.0 - nonScaledToScaledRatio) / 0.5;
                }
                Acceleration = Math.Max(Math.Min(Acceleration, 1.0), -1.0);
            }
            if (SkillId != SkillItem.ResurrectId)
            {
                switch (endItem.IsActivation)
                {
                    case ArcDPSEnums.Activation.CancelCancel:
                        Status = AnimationStatus.Interrupted;
                        SavedDuration = -ActualDuration;
                        break;
                    case ArcDPSEnums.Activation.Reset:
                        Status = AnimationStatus.Full;
                        break;
                    case ArcDPSEnums.Activation.CancelFire:
                        int nonScaledExpectedDuration = (int)Math.Round(ExpectedDuration / nonScaledToScaledRatio);
                        SavedDuration = Math.Max(nonScaledExpectedDuration - ActualDuration, 0);
                        Status = AnimationStatus.Reduced;
                        break;
                }
            }
            Acceleration = Math.Round(Acceleration, ParserHelper.AccelerationDigit);
        }

        internal AnimatedCastEvent(AgentData agentData, SkillData skillData, CombatItem endItem) : base (endItem, agentData, skillData)
        {
            ActualDuration = endItem.Value;
            ExpectedDuration = ActualDuration;
            Time -= ActualDuration;
            _scaledActualDuration = endItem.BuffDmg;
            if (Skill.ID == SkillItem.DodgeId)
            {
                ExpectedDuration = 750;
                ActualDuration = 750;
                _scaledActualDuration = 0;
            }
            SetAcceleration(endItem);
        }

        internal AnimatedCastEvent(CombatItem startItem, AgentData agentData, SkillData skillData, CombatItem endItem) : this(startItem, agentData, skillData)
        {
            ActualDuration = endItem.Value;
            _scaledActualDuration = endItem.BuffDmg;
            if (Skill.ID == SkillItem.DodgeId)
            {
                ExpectedDuration = 750;
                ActualDuration = 750;
                _scaledActualDuration = 0;
            }
            SetAcceleration(endItem);
        }

        internal AnimatedCastEvent(CombatItem startItem, AgentData agentData, SkillData skillData, long maxEnd) : this(startItem, agentData, skillData)
        {
            if (Skill.ID == SkillItem.DodgeId)
            {
                ExpectedDuration = 750;
            }
            ActualDuration = ExpectedDuration;
            if (ActualDuration + Time > maxEnd)
            {
                ActualDuration = (int)(maxEnd - Time);
            }
        }
    }
}
