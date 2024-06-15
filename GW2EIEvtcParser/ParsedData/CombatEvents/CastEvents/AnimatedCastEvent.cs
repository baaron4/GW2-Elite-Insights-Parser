using System;
using System.Linq;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class AnimatedCastEvent : AbstractCastEvent
    {
        private readonly int _scaledActualDuration;
        //private readonly int _effectHappenedDuration;

        public Point3D EffectPosition { get; }

        public bool HasEffectPosition => EffectPosition != null;

        private AnimatedCastEvent(CombatItem startItem, AgentData agentData, SkillData skillData) : base(startItem, agentData, skillData)
        {
            ExpectedDuration = startItem.BuffDmg > 0 ? startItem.BuffDmg : startItem.Value;
            if (startItem.IsActivation == ArcDPSEnums.Activation.Quickness)
            {
                Acceleration = 1;
            }
            if (startItem.DstAgent != 0 || startItem.OverstackValue != 0)
            {
                byte[] xyBytes = BitConverter.GetBytes(startItem.DstAgent);
                byte[] zBytes = BitConverter.GetBytes(startItem.OverstackValue);
                EffectPosition = new Point3D(BitConverter.ToSingle(xyBytes, 0), BitConverter.ToSingle(xyBytes, 4), BitConverter.ToSingle(zBytes, 0));
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
                    Acceleration = (nonScaledToScaledRatio - 1.0) / 0.5;
                }
                else
                {
                    Acceleration = -(1.0 - nonScaledToScaledRatio) / 0.6;
                }
                Acceleration = Math.Max(Math.Min(Acceleration, 1.0), -1.0);
            }
            if (SkillId != SkillIDs.Resurrect)
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
                        int scaledExpectedDuration = (int)Math.Round(ExpectedDuration / nonScaledToScaledRatio);
                        SavedDuration = Math.Max(scaledExpectedDuration - ActualDuration, 0);
                        Status = AnimationStatus.Reduced;
                        break;
                }
            }
            Acceleration = Math.Round(Acceleration, ParserHelper.AccelerationDigit);
        }

        // Start missing
        internal AnimatedCastEvent(AgentData agentData, SkillData skillData, CombatItem endItem) : base(endItem, agentData, skillData)
        {
            ActualDuration = endItem.Value;
            ExpectedDuration = ActualDuration;
            _scaledActualDuration = endItem.BuffDmg;
            if (Skill.IsAnimatedDodge(skillData))
            {
                // dodge animation start item has always 0 as expected duration
                ExpectedDuration = ActualDuration;
                _scaledActualDuration = 0;
            }
            Time -= ActualDuration;
            SetAcceleration(endItem);
        }

        // Start and End both present
        internal AnimatedCastEvent(CombatItem startItem, AgentData agentData, SkillData skillData, CombatItem endItem) : this(startItem, agentData, skillData)
        {
            ActualDuration = endItem.Value;
            _scaledActualDuration = endItem.BuffDmg;
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

        // End missing
        internal AnimatedCastEvent(CombatItem startItem, AgentData agentData, SkillData skillData, long maxEnd) : this(startItem, agentData, skillData)
        {
            if (Skill.ID == skillData.DodgeId)
            {
                // TODO: vindicator dodge duration
                ExpectedDuration = 750;
            }
            ActualDuration = ExpectedDuration;
            CutAt(maxEnd);
        }

        internal void CutAt(long maxEnd)
        {
            if (EndTime > maxEnd && Status == AnimationStatus.Unknown)
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

        public override long GetInterruptedByStunTime(ParsedEvtcLog log)
        {
            Segment stunStatus = log.FindActor(Caster).GetBuffStatus(log, SkillIDs.Stun, Time, ExpectedEndTime).FirstOrDefault(x => x.Value > 0);
            if (stunStatus != null)
            {
                return (int)stunStatus.Start;
            }
            return EndTime;
        }
    }
}
