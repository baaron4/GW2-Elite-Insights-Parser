using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class InstantCastFinder : IVersionable
    {
        public const long DefaultICD = 50;
        public long SkillID { get; }

        protected long ICD { get; }

        private ulong _maxBuild { get; } = ulong.MaxValue;
        private ulong _minBuild { get; } = ulong.MinValue;

        protected InstantCastFinder(long skillID, long icd)
        {
            SkillID = skillID;
            ICD = icd;
        }

        protected InstantCastFinder(long skillID, long icd, ulong minBuild, ulong maxBuild) : this(skillID, icd)
        {
            _maxBuild = maxBuild;
            _minBuild = minBuild;
        }


        public bool Available(ulong gw2Build)
        {
            return gw2Build < _maxBuild && gw2Build >= _minBuild;
        }

        public abstract List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData);
    }
}
