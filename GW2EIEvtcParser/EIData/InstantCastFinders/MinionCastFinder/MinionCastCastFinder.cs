using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class MinionCastCastFinder : CheckedCastFinder<AnimatedCastEvent>
    {
        protected long MinionSkillID { get; }

        public MinionCastCastFinder(long skillID, long minionSkillID) : base(skillID)
        {
            MinionSkillID = minionSkillID;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var result = new List<InstantCastEvent>();
            var casts = combatData.GetAnimatedCastData(MinionSkillID)
                .Where(x => x.Caster.Master != null)
                .GroupBy(x => x.Caster)
                .ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<AnimatedCastEvent>> pair in casts)
            {
                long lastTime = int.MinValue;
                foreach (AnimatedCastEvent cast in pair.Value)
                {
                    if (CheckCondition(cast, combatData, agentData, skillData))
                    {
                        if (cast.Time - lastTime < ICD)
                        {
                            lastTime = cast.Time;
                            continue;
                        }
                        result.Add(new InstantCastEvent(cast.Time, skillData.Get(SkillID), cast.Caster.GetFinalMaster()));
                    }
                }
            }
            return result;
        }
    }
}
