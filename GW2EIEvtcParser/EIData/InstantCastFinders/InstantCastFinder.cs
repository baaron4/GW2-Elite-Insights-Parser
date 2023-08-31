using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Interfaces;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class InstantCastFinder : IVersionable
    {

        public enum InstantCastOrigin
        {
            Skill,
            Trait,
            Gear
        }

        public InstantCastOrigin CastOrigin { get; private set; } = InstantCastOrigin.Skill;

        public delegate bool InstantCastEnableChecker(CombatData combatData);
        private List<InstantCastEnableChecker> _enableConditions { get; }


        public const long DefaultICD = 50;
        public long SkillID { get; }

        public bool NotAccurate { get; private set; } = false;

        protected long TimeOffset { get; set; } = 0;

        protected bool BeforeWeaponSwap { get; set; } = false;
        protected bool AfterWeaponSwap { get; set; } = false;

        protected long ICD { get; private set; } = DefaultICD;

        private ulong _maxBuild { get; set; } = GW2Builds.EndOfLife;
        private ulong _minBuild { get; set; } = GW2Builds.StartOfLife;

        protected InstantCastFinder(long skillID)
        {
            SkillID = skillID;
            _enableConditions = new List<InstantCastEnableChecker>();
        }

        internal InstantCastFinder WithBuilds(ulong minBuild, ulong maxBuild = GW2Builds.EndOfLife)
        {
            _maxBuild = maxBuild;
            _minBuild = minBuild;
            return this;
        }

        internal InstantCastFinder UsingICD(long icd = DefaultICD)
        {
            ICD = icd;
            return this;
        }

        internal InstantCastFinder UsingNotAccurate(bool notAccurate)
        {
            NotAccurate = notAccurate;
            return this;
        }

        internal InstantCastFinder UsingOrigin(InstantCastOrigin origin)
        {
            CastOrigin = origin;
            return this;
        }

        internal InstantCastFinder UsingEnable(InstantCastEnableChecker checker)
        {
            _enableConditions.Add(checker);
            return this;
        }

        internal InstantCastFinder UsingDisableWithEffectData()
        {
            return UsingEnable(combatData => !combatData.HasEffectData);
        }

        internal virtual InstantCastFinder UsingTimeOffset(long timeOffset)
        {
            TimeOffset = timeOffset;
            return this;
        }

        internal virtual InstantCastFinder UsingBeforeWeaponSwap(bool beforeWeaponSwap)
        {
            BeforeWeaponSwap = beforeWeaponSwap;
            AfterWeaponSwap = false;
            return this;
        }

        internal virtual InstantCastFinder UsingAfterWeaponSwap(bool afterWeaponSwap)
        {
            AfterWeaponSwap = afterWeaponSwap;
            BeforeWeaponSwap = false;
            return this;
        }

        protected long GetTime(AbstractTimeCombatEvent evt, AgentItem caster, CombatData combatData)
        {
            long time = evt.Time +  TimeOffset;
            if (BeforeWeaponSwap || AfterWeaponSwap)
            {
                var wepSwaps = combatData.GetWeaponSwapData(caster).Where(x => Math.Abs(x.Time - time) < ServerDelayConstant / 2).ToList();
                if (wepSwaps.Any())
                {
                    return BeforeWeaponSwap ? Math.Min(wepSwaps[0].Time - 1, time) : Math.Max(wepSwaps[0].Time + 1, time);
                }
            }
            return time;
        }


        public bool Available(CombatData combatData)
        {
            if (!_enableConditions.All(checker => checker(combatData)))
            {
                return false;
            }
            ulong gw2Build = combatData.GetBuildEvent().Build;
            return gw2Build < _maxBuild && gw2Build >= _minBuild;
        }

        public abstract List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData);
    }
}
