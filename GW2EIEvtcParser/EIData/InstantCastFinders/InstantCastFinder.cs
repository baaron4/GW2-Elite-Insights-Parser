using System.Collections.Generic;
using GW2EIEvtcParser.Interfaces;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class InstantCastFinder : IVersionable
    {
        public delegate bool InstantCastEnableChecker(CombatData combatData);
        private InstantCastEnableChecker _enableCondition { get; set; }
        private InstantCastEnableChecker _enableConditionInternal { get; set; }


        public const long DefaultICD = 50;
        public long SkillID { get; }

        public bool NotAccurate { get; private set; } = false;

        protected long ICD { get; private set; } = DefaultICD;

        private ulong _maxBuild { get; set; } = GW2Builds.EndOfLife;
        private ulong _minBuild { get; set; } = GW2Builds.StartOfLife;

        protected InstantCastFinder(long skillID)
        {
            SkillID = skillID;
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

        internal InstantCastFinder UsingEnable(InstantCastEnableChecker checker)
        {
            _enableCondition = checker;
            return this;
        }

        protected InstantCastFinder UsingEnableInternal(InstantCastEnableChecker checker)
        {
            _enableConditionInternal = checker;
            return this;
        }


        public bool Available(CombatData combatData)
        {
            if (_enableConditionInternal != null && !_enableConditionInternal(combatData))
            {
                return false;
            }
            if (_enableCondition != null && !_enableCondition(combatData)) {
                return false;
            }
            ulong gw2Build = combatData.GetBuildEvent().Build;
            return gw2Build < _maxBuild && gw2Build >= _minBuild;
        }

        public abstract List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData);
    }
}
