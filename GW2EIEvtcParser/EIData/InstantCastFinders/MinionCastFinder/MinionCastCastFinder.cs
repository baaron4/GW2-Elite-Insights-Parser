using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class MinionCastCastFinder : CheckedCastFinder<AnimatedCastEvent>
{
    protected readonly long MinionSkillID;

    public MinionCastCastFinder(long skillID, long minionSkillID) : base(skillID)
    {
        MinionSkillID = minionSkillID;
    }

    public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
    {
        var result = new List<InstantCastEvent>(); //TODO_PERF(Rennorb)
        var casts = combatData.GetAnimatedCastData(MinionSkillID)
            .Where(x => x.Caster.Master != null)
            .GroupBy(x => x.Caster);
        foreach (var group in casts)
        {
            long lastTime = int.MinValue;
            foreach (AnimatedCastEvent cast in group)
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
